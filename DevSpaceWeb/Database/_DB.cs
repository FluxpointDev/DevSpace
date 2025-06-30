using DaRT;
using DevSpaceWeb.Apps;
using DevSpaceWeb.Apps.Data;
using DevSpaceWeb.Data;
using DevSpaceWeb.Data.API;
using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Data.Websites;
using Discord.Rest;
using LibMCRcon.RCon;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net;

namespace DevSpaceWeb.Database;

#pragma warning disable IDE1006 // Naming Styles
public static class _DB
#pragma warning restore IDE1006 // Naming Styles
{
    public static ConfigDatabase? Configure;

    public static MongoClient Client = null!;

    public static bool IsConnected;

    public static bool HasException;

    public static event SessionEventHandler? SessionUpdated;

    public static void TriggerSessionEvent(ObjectId user, SessionEventType type)
    {
        SessionUpdated?.Invoke(user, type);
    }

    public static void Init()
    {
        ConfigDatabase? config = null;
        if (Program.IsDevMode)
        {
            if (File.Exists(Program.Directory.Data.Path + "Dev.json"))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(Program.Directory.Data.Path + "Dev.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        };
                        config = (ConfigDatabase?)serializer.Deserialize(reader, typeof(ConfigDatabase));
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogMessage("Failed to parse Dev.json file, " + ex.Message, LogSeverity.Error);
                }

            }

            if (config == null)
            {
                Logger.LogMessage("Failed to load Dev.json file.", LogSeverity.Error);
            }
        }
        else
        {
            if (!File.Exists(Program.Directory.Data.Path + "Database.json"))
            {
                config = new ConfigDatabase();
                using (StreamWriter file = File.CreateText(Program.Directory.Data.Path + "Database.json"))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        Formatting = Formatting.Indented
                    };
                    serializer.Serialize(file, config);
                }
            }
            else
            {
                try
                {
                    using (StreamReader reader = new StreamReader(Program.Directory.Data.Path + "Database.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        };
                        config = (ConfigDatabase?)serializer.Deserialize(reader, typeof(ConfigDatabase));
                    }

                }
                catch (Exception ex)
                {
                    Logger.LogMessage("Failed to parse Database.json file, " + ex.Message, LogSeverity.Error);
                }
            }

            if (config == null)
                throw new ArgumentException("Failed to load Database.json file.");

        }

        Configure = config;
        Configure.Database.ConnectionString = Configure.GetConnectionString();

        if (string.IsNullOrEmpty(Configure.Database.ConnectionString))
        {
            if (string.IsNullOrEmpty(Configure.Database.Host))
                throw new ArgumentException("Database host not configured in appsettings.json");

            if (Configure.Database.Port == 0)
                throw new ArgumentException("Database port not configured in appsettings.json");

            if (string.IsNullOrEmpty(Configure.Database.Name))
                throw new ArgumentException("Database name not configured in appsettings.json");

            if (string.IsNullOrEmpty(Configure.Database.User))
                throw new ArgumentException("Database user not configured in appsettings.json");
        }

        Client = new MongoClient(Configure.GetConnectionString());

        Run = Client.GetDatabase(Configure.Database.Name);
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
        Apps = new ICacheCollection<AppData>("apps");
        Workspaces = new ICollection<WorkspaceData>("workspaces");
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
                if (x.Proxmox != null && !string.IsNullOrEmpty(x.Proxmox.Secret))
                    _Data.ProxmoxAgents.Add(x.Id, new Data.Proxmox.ProxmoxAgent(x.Proxmox));
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
        catch
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
#pragma warning disable CS0618 // Type or member is obsolete
                            if (i.Roles != null)
                            {
                                _ = i.UpdateAsync(new UpdateDefinitionBuilder<TeamData>().Unset(x => x.Roles), () =>
                                {

                                });
                            }


                            int Position = 0;
                            Dictionary<ObjectId, int> UpdatedPositions = [];
                            foreach (TeamRoleData? r in i.CachedRoles.Values.OrderBy(x => x.Position.GetValueOrDefault()))
                            {

                                UpdatedPositions.Add(r.Id, Position);
                                Position += 1;
                                _ = r.UpdateAsync(new UpdateDefinitionBuilder<TeamRoleData>().Unset(x => x.Position), () =>
                                {

                                });
                            }
#pragma warning restore CS0618 // Type or member is obsolete
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

        Task AppTask = Task.Run(async () =>
        {
            try
            {
                await Apps.Find(Builders<AppData>.Filter.Empty).ForEachAsync(async x =>
                {
                    Apps.Cache.TryAdd(x.Id, x);

                    bool IsDev = Program.IsDevMode || Program.IsPreviewMode;
                    if (!IsDev || x.OwnerId.ToString() == "6757b63be964c430187491bb")
                    {
                        _Data.DiscordClients.Add(x.Id, new DiscordRestClient(new DiscordRestConfig
                        {
                            //RestClientProvider = IsDev ? DefaultRestClientProvider.Create(new WebProxy("http://127.0.0.1", 8888)) : DefaultRestClientProvider.Instance,
                            APIOnRestInteractionCreation = false,
                            UseInteractionSnowflakeDate = false
                        }));
                        try
                        {
                            await _Data.DiscordClients[x.Id].LoginAsync(Discord.TokenType.Bot, Crypt.DecryptString(x.EncryptedToken));
                        }
                        catch { }
                    }
                });


                Logger.LogMessage("Database", "- Apps: " + Apps.Cache.Keys.Count, LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Database", "- Apps: FAIL!", LogSeverity.Info);
                Console.WriteLine(ex);
                HasException = true;
            }
        });

        Task.WaitAll(RoleTask, MemberTask, TeamVanityTask, TemplateTask, LogTask,
            ServerTask, WebsiteTask, ProjectTask, APITask, ConsoleTask, AppTask);

        if (HasException)
            return false;

        Task WorkspaceTask = Task.Run(async () =>
        {
            try
            {
                await Workspaces.Find(Builders<WorkspaceData>.Filter.Empty).ForEachAsync(x =>
                {
                    if (!string.IsNullOrEmpty(x.JsonData))
                    {
                        if (_DB.Apps.Cache.TryGetValue(x.AppId, out AppData? app))
                        {
                            RequestBlocks? Command = Newtonsoft.Json.JsonConvert.DeserializeObject<RequestBlocks>(x.JsonData, new Newtonsoft.Json.JsonSerializerSettings
                            {
                                MaxDepth = 512
                            });
                            if (Command != null)
                            {
                                app.GetCache().AddCommandCache(x.Id, Command.blocks.blocks[0]);
                            }
                        }
                    }
                });
                Logger.LogMessage("Database", "- Workspaces: Loaded.", LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Database", "- Workspaces: FAIL!", LogSeverity.Info);
                Console.WriteLine(ex);
            }
        });

        Logger.LogMessage("Database", "Data Loaded", LogSeverity.Info);
        return true;
    }

    public static IMongoDatabase Run = null!;

    public static ICacheCollection<TeamData> Teams = null!;

    public static ConcurrentDictionary<string, TeamData> TeamsVanityCache = new ConcurrentDictionary<string, TeamData>();
    public static ConcurrentDictionary<ObjectId, string> VanityUrlCache = new ConcurrentDictionary<ObjectId, string>();

    public static ICacheCollection<ServerData> Servers = null!;

    public static Dictionary<ObjectId, PartialUserData> Users = [];

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

    public static ICacheCollection<AppData> Apps = null!;

    public static ICollection<WorkspaceData> Workspaces = null!;
}
