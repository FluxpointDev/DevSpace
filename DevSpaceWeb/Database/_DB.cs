using DaRT;
using DevSpaceWeb.Data;
using DevSpaceWeb.Data.API;
using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Data.Websites;
using LibMCRcon.RCon;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net;

namespace DevSpaceWeb.Database;

public static class _DB
{
    public static ConfigureDatabase? Configure;

    public static MongoClient Client = null!;

    public static bool IsConnected;

    public static bool HasException;

    public static event SessionEventHandler? SessionUpdated;

    public static void TriggerSessionEvent(ObjectId user, SessionEventType type)
    {
        SessionUpdated?.Invoke(user, type);
    }

    public static void Init(ConfigurationManager configuration)
    {
        if (Program.IsUsingAspire)
        {
            Configure = new ConfigureDatabase
            {
                ConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__dev-space-db"),
                Name = Environment.GetEnvironmentVariable("DB_NAME")
            };
        }
        else if (Program.IsDevMode)
        {
            if (!File.Exists(Program.Directory.Data.Path + "Dev.json"))
            {
                using (StreamWriter file = File.CreateText(Program.Directory.Data.Path + "Dev.json"))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        Formatting = Formatting.Indented
                    };
                    serializer.Serialize(file, new ConfigureDatabase());
                }
            }
            else
            {
                ConfigureDatabase? config = null;
                try
                {
                    using (StreamReader reader = new StreamReader(Program.Directory.Data.Path + "Dev.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        };
                        config = (ConfigureDatabase?)serializer.Deserialize(reader, typeof(ConfigureDatabase));
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogMessage("Failed to parse Dev.json file, " + ex.Message, LogSeverity.Error);
                }

                if (config == null)
                {
                    Logger.LogMessage("Failed to load Dev.json file.", LogSeverity.Error);
                }

                Configure = config;
            }
        }
        else
        {
            Configure = new ConfigureDatabase
            {
                Host = configuration.GetSection("Database").GetValue<string>("Host"),
                Port = configuration.GetSection("Database").GetValue<short>("Port"),
                Name = configuration.GetSection("Database").GetValue<string>("Name"),
                User = configuration.GetSection("Database").GetValue<string>("User"),
                Password = configuration.GetSection("Database").GetValue<string>("Password"),
            };
        }

        if (Configure == null)
            throw new Exception("Database configuration is invalid.");

        if (String.IsNullOrEmpty(Configure.ConnectionString))
        {
            if (string.IsNullOrEmpty(Configure.Host))
                throw new ArgumentException("Database host not configured in appsettings.json");

            if (Configure.Port == 0)
                throw new ArgumentException("Database port not configured in appsettings.json");

            if (string.IsNullOrEmpty(Configure.Name))
                throw new ArgumentException("Database name not configured in appsettings.json");

            if (string.IsNullOrEmpty(Configure.User))
                throw new ArgumentException("Database user not configured in appsettings.json");
        }

        Client = new MongoClient(Configure.GetConnectionString());

        Run = Client.GetDatabase(Configure.Name);
        Teams = new ICacheCollection<TeamData>("teams");
        Roles = new ICacheCollection<TeamRoleData>("roles");
        Members = new ICacheCollection<TeamMemberData>("members");
        Servers = new ICacheCollection<ServerData>("servers");
        Consoles = new ICacheCollection<ConsoleData>("consoles");
        Projects = new ICacheCollection<ProjectData>("projects");
        Websites = new ICacheCollection<WebsiteData>("websites");
        Logs = new ICacheCollection<LogData>("logs");
        EmailTemplates = new ICacheCollection<EmailTemplateData>("email_templates");
        TeamVanityUrls = new ICacheCollection<VanityUrlData>("vanity_urls");
        AuditLogs = new ICollection<AuditLog>("audit");
        API = new ICacheCollection<APIClient>("api");
        Notifications = new ICollection<Notification>("notifications");
    }

    public static async Task<bool> StartAsync()
    {
        try
        {
            BsonDocument result = await Run.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
            Logger.LogMessage("Database", "Pinged your deployment. You successfully connected to MongoDB!", LogSeverity.Info);
        }
        catch { return false; }

        Logger.LogMessage("Database", "Loading...", LogSeverity.Info);
        try
        {
            await Teams.Find(Builders<TeamData>.Filter.Empty).ForEachAsync(x =>
            {
                Teams.Cache.TryAdd(x.Id, x);
                if (!string.IsNullOrEmpty(x.VanityUrl))
                {
                    TeamsVanityCache.TryAdd(x.VanityUrl, x);
                    VanityUrlCache.TryAdd(x.Id, x.VanityUrl);
                }
            });
            Logger.LogMessage("Database", "- Teams: " + Teams.Cache.Keys.Count, LogSeverity.Info);
        }
        catch (Exception ex)
        {
            HasException = true;
            Logger.LogMessage("Database", "- Teams: FAIL!", LogSeverity.Info);
            Console.WriteLine(ex);
            return false;
        }

        try
        {
#pragma warning disable CS0618 // Type or member is obsolete
            _ = Run.GetCollection<AuditLog>("audit").Indexes.CreateOne(Builders<AuditLog>.IndexKeys.Ascending("CreatedAt"),
                new CreateIndexOptions { ExpireAfter = new TimeSpan(30, 0, 0, 0), Name = "_expire_at_" });
#pragma warning restore CS0618 // Type or member is obsolete
        }
        catch (Exception ex)
        {

        }

        try
        {
            await Run.GetCollection<AuthUser>("users").Find(Builders<AuthUser>.Filter.Empty).ForEachAsync(x =>
            {
                Users.Add(x.Id, new PartialUserData(x));
            });
            Logger.LogMessage("Database", "- Users: " + Users.Keys.Count, LogSeverity.Info);
        }
        catch (Exception ex)
        {
            HasException = true;
            Logger.LogMessage("Database", "- Users: FAIL!", LogSeverity.Info);
            Console.WriteLine(ex);

            return false;
        }



        Task RoleTask = Task.Run(async () =>
        {
            try
            {
                await Roles.Find(Builders<TeamRoleData>.Filter.Empty).ForEachAsync(x =>
                {
                    if (Roles.Cache.TryAdd(x.Id, x))
                    {
                        if (Teams.Cache.TryGetValue(x.TeamId, out TeamData? team))
                            team.CachedRoles.Add(x.Id, x);
                    }
                });
                Logger.LogMessage("Database", "- Roles: " + Roles.Cache.Keys.Count, LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Database", "- Roles: FAIL!", LogSeverity.Info);
                Console.WriteLine(ex);
                HasException = true;
            }
            if (!HasException)
            {
                try
                {
                    // Migrate/Fix roles and role positions
                    foreach (TeamData i in Teams.Cache.Values)
                    {
                        if (i.CachedRoles.Count != 0 && i.RolePositions.Count == 0)
                        {
                            Logger.LogMessage("Database", "- Roles: Migrating " + i.Name, LogSeverity.Info);

#pragma warning disable CS0612 // Type or member is obsolete
                            if (i.Roles != null)
                            {
                                _ = i.UpdateAsync(new UpdateDefinitionBuilder<TeamData>().Unset(x => x.Roles), () =>
                                {

                                });
                            }

                            int Position = 0;
                            Dictionary<ObjectId, int> UpdatedPositions = new Dictionary<ObjectId, int>();
                            foreach (TeamRoleData? r in i.CachedRoles.Values.OrderBy(x => x.Position.GetValueOrDefault()))
                            {

                                UpdatedPositions.Add(r.Id, Position);
                                Position += 1;
                                _ = r.UpdateAsync(new UpdateDefinitionBuilder<TeamRoleData>().Unset(x => x.Position), () =>
                                {

                                });
                            }
#pragma warning restore CS0612 // Type or member is obsolete

                            await i.UpdateAsync(new UpdateDefinitionBuilder<TeamData>().Set(x => x.RolePositions, UpdatedPositions), () =>
                            {

                            });
                            Logger.LogMessage("Database", "- Roles: Migration done " + i.Name, LogSeverity.Info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogMessage("Database", "- Roles: MIGRATION FAIL!", LogSeverity.Info);
                    Console.WriteLine(ex);
                    HasException = true;
                }
            }
        });

        Task MemberTask = Task.Run(async () =>
        {
            try
            {
                await Members.Find(Builders<TeamMemberData>.Filter.Empty).ForEachAsync(async x =>
                {
                    // Fix joined date for old members data
                    if (x.JoinedAt != x.Id.CreationTime)
                        x.JoinedAt = x.Id.CreationTime;

                    if (Members.Cache.TryAdd(x.Id, x))
                    {
                        if (Teams.Cache.TryGetValue(x.TeamId, out TeamData? team))
                        {
                            // Fix invalid team members
                            if (team.Members.Count == 0)
                            {
                                team.Members.Add(x.UserId, x.Id);
                                await team.UpdateAsync(new UpdateDefinitionBuilder<TeamData>().Set(x => x.Members, team.Members));
                            }

                            team.CachedMembers.Add(x.Id, x);
                        }
                    }
                });
                Logger.LogMessage("Database", "- Members: " + Members.Cache.Keys.Count, LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Database", "- Members: FAIL!", LogSeverity.Info);
                Console.WriteLine(ex);
                HasException = true;
            }
        });

        Task TeamVanityTask = Task.Run(async () =>
        {
            try
            {
                await TeamVanityUrls.Find(Builders<VanityUrlData>.Filter.Empty).ForEachAsync(x =>
                {
                    if (TeamVanityUrls.Cache.TryAdd(x.Id, x))
                    {
                        foreach (KeyValuePair<string, ObjectId> i in x.ServerVanityUrls)
                        {
                            VanityUrlCache.TryAdd(i.Value, i.Key);
                        }
                        foreach (KeyValuePair<string, ObjectId> i in x.LogVanityUrls)
                        {
                            VanityUrlCache.TryAdd(i.Value, i.Key);
                        }
                        foreach (KeyValuePair<string, ObjectId> i in x.ProjectVanityUrls)
                        {
                            VanityUrlCache.TryAdd(i.Value, i.Key);
                        }
                        foreach (KeyValuePair<string, ObjectId> i in x.ConsoleVanityUrls)
                        {
                            VanityUrlCache.TryAdd(i.Value, i.Key);
                        }
                        foreach (KeyValuePair<string, ObjectId> i in x.WebsiteVanityUrls)
                        {
                            VanityUrlCache.TryAdd(i.Value, i.Key);
                        }
                    }
                });
                Logger.LogMessage("Database", "- Vanity URLs: " + TeamVanityUrls.Cache.Keys.Count, LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Database", "- Vanity URLs: FAIL!", LogSeverity.Info);
                Console.WriteLine(ex);
                HasException = true;
            }
        });


        Task TemplateTask = Task.Run(async () =>
        {
            if (Program.LimitMode)
                return;

            try
            {
                await EmailTemplates.Find(Builders<EmailTemplateData>.Filter.Empty).ForEachAsync(x =>
                {
                    EmailTemplates.Cache.TryAdd(x.Id, x);
                });
                Logger.LogMessage("Database", "- Email Templates: " + EmailTemplates.Cache.Keys.Count, LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Database", "- Email Templates: FAIL!", LogSeverity.Info);
                Console.WriteLine(ex);
                HasException = true;
            }
        });

        Task LogTask = Task.Run(async () =>
        {
            if (Program.LimitMode)
                return;

            try
            {
                await Logs.Find(Builders<LogData>.Filter.Empty).ForEachAsync(x =>
                {
                    Logs.Cache.TryAdd(x.Id, x);
                });
                Logger.LogMessage("Database", "- Logs: " + Logs.Cache.Keys.Count, LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Database", "- Logs: FAIL!", LogSeverity.Info);
                Console.WriteLine(ex);
                HasException = true;
            }
        });

        Task ServerTask = Task.Run(async () =>
        {
            try
            {
                await Servers.Find(Builders<ServerData>.Filter.Empty).ForEachAsync(x =>
                {
                    Servers.Cache.TryAdd(x.Id, x);
                    bool IsDev = Program.IsDevMode || Program.IsPreviewMode;
                    if (!IsDev || x.OwnerId.ToString() == "6757b63be964c430187491bb")
                        _ = x.StartWebSocket(true);
                });
                Logger.LogMessage("Database", "- Servers: " + Servers.Cache.Keys.Count, LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Database", "- Servers: FAIL!", LogSeverity.Info);
                Console.WriteLine(ex);
                HasException = true;
            }
        });

        Task WebsiteTask = Task.Run(async () =>
        {
            if (Program.LimitMode)
                return;
            try
            {
                await Websites.Find(Builders<WebsiteData>.Filter.Empty).ForEachAsync(x =>
                {
                    Websites.Cache.TryAdd(x.Id, x);
                });
                Logger.LogMessage("Database", "- Websites: " + Websites.Cache.Keys.Count, LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Database", "- Websites: FAIL!", LogSeverity.Info);
                Console.WriteLine(ex);
                HasException = true;
            }
        });

        Task ProjectTask = Task.Run(async () =>
        {
            if (Program.LimitMode)
                return;
            try
            {
                await Projects.Find(Builders<ProjectData>.Filter.Empty).ForEachAsync(x =>
                {
                    Projects.Cache.TryAdd(x.Id, x);
                });
                Logger.LogMessage("Database", "- Projects: " + Projects.Cache.Keys.Count, LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Database", "- Projects: FAIL!", LogSeverity.Info);
                Console.WriteLine(ex);
                HasException = true;
            }
        });

        Task ConsoleTask = Task.Run(async () =>
        {
            if (Program.LimitMode)
                return;
            try
            {
                await Consoles.Find(Builders<ConsoleData>.Filter.Empty).ForEachAsync(x =>
                {
                    Consoles.Cache.TryAdd(x.Id, x);
                    switch (x.Type)
                    {
                        case ConsoleType.Battleye:
                            {
                                RCon rc = new RCon();
                                rc.Connect(IPAddress.Parse(x.Ip), x.Port, x.GetDecryptedPassword());
                                if (rc.IsConnected)
                                    x.ConnectedAt = DateTime.UtcNow;
                                _Data.BattleyeRcons.Add(x.Id, rc);
                            }
                            break;
                        case ConsoleType.Minecraft:
                            {
                                TCPRconAsync rcon = new TCPRconAsync
                                {
                                    RConHost = x.Ip,
                                    RConPort = x.Port,
                                    RConPass = x.GetDecryptedPassword()
                                };
                                _ = rcon.StartComms();
                                _Data.MinecraftRcons.Add(x.Id, rcon);
                            }
                            break;
                    }
                });
                Logger.LogMessage("Database", "- Consoles: " + Consoles.Cache.Keys.Count, LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Database", "- Consoles: FAIL!", LogSeverity.Info);
                Console.WriteLine(ex);
                HasException = true;
            }
        });

        Task APITask = Task.Run(async () =>
        {
            try
            {
                await API.Find(Builders<APIClient>.Filter.Empty).ForEachAsync(x =>
                {
                    API.Cache.TryAdd(x.Id, x);
                });
                Logger.LogMessage("Database", "- APIs: " + API.Cache.Keys.Count, LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Database", "- APIs: FAIL!", LogSeverity.Info);
                Console.WriteLine(ex);
                HasException = true;
            }
        });

        Task.WaitAll(RoleTask, MemberTask, TeamVanityTask, TemplateTask, LogTask,
            ServerTask, WebsiteTask, ProjectTask, APITask, ConsoleTask);

        if (HasException)
            return false;

        Logger.LogMessage("Database", "Data Loaded", LogSeverity.Info);
        return true;
    }

    public static IMongoDatabase Run = null!;

    public static ICacheCollection<TeamData> Teams = null!;

    public static ConcurrentDictionary<string, TeamData> TeamsVanityCache = new ConcurrentDictionary<string, TeamData>();
    public static ConcurrentDictionary<ObjectId, string> VanityUrlCache = new ConcurrentDictionary<ObjectId, string>();

    public static ICacheCollection<ServerData> Servers = null!;

    public static Dictionary<ObjectId, PartialUserData> Users = new Dictionary<ObjectId, PartialUserData>();

    public static ICacheCollection<ConsoleData> Consoles = null!;

    public static ICacheCollection<ProjectData> Projects = null!;

    public static ICacheCollection<WebsiteData> Websites = null!;

    public static ICacheCollection<LogData> Logs = null!;

    public static ICacheCollection<TeamRoleData> Roles = null!;

    public static ICacheCollection<TeamMemberData> Members = null!;

    public static ICacheCollection<VanityUrlData> TeamVanityUrls = null!;

    public static ICacheCollection<EmailTemplateData> EmailTemplates = null!;

    public static ICollection<AuditLog> AuditLogs = null!;

    public static ICacheCollection<APIClient> API = null!;

    public static ICollection<Notification> Notifications = null!;
}
