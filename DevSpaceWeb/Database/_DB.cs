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
using System.Collections.Concurrent;
using System.Net;

namespace DevSpaceWeb.Database;

public static class _DB
{
    public static MongoClient Client = null!;

    public static bool IsConnected;
    public static bool IsRestartRequired = false;
    public static bool IsCacheDone;
    public static bool HasException;

    public static event SessionEventHandler SessionUpdated;

    public static void TriggerSessionEvent(ObjectId user, SessionEventType type)
    {
        SessionUpdated?.Invoke(null, user, type);
    }

    public static void Init()
    {
        if (Run == null)
        {
            Run = Client.GetDatabase(_Data.Config.Database.Name);
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
        }
    }

    public static async Task<bool> StartAsync()
    {
        try
        {
            BsonDocument result = await Run.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
            IsConnected = true;
            Logger.LogMessage("Database", "Pinged your deployment. You successfully connected to MongoDB!", LogSeverity.Info);
        }
        catch (Exception ex) { Console.WriteLine(ex); return false; }

        if (!IsCacheDone)
        {
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
                            if (Teams.Cache.TryGetValue(x.TeamId, out TeamData team))
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
                        foreach (var i in Teams.Cache.Values)
                        {
                            if (i.CachedRoles.Any() && !i.RolePositions.Any())
                            {
                                Logger.LogMessage("Database", "- Roles: Migrating " + i.Name, LogSeverity.Info);

                                if (i.Roles != null)
                                {
                                    _ = i.UpdateAsync(new UpdateDefinitionBuilder<TeamData>().Unset(x => x.Roles), () =>
                                    {

                                    });
                                }

                                int Position = 0;
                                Dictionary<ObjectId, int> UpdatedPositions = new Dictionary<ObjectId, int>();
                                foreach (var r in i.CachedRoles.Values.OrderBy(x => x.Position.GetValueOrDefault()))
                                {

                                    UpdatedPositions.Add(r.Id, Position);
                                    Position += 1;
                                    _ = r.UpdateAsync(new UpdateDefinitionBuilder<TeamRoleData>().Unset(x => x.Position), () =>
                                    {

                                    });
                                }
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
                    await Members.Find(Builders<TeamMemberData>.Filter.Empty).ForEachAsync(x =>
                    {
                        if (Members.Cache.TryAdd(x.Id, x))
                        {
                            if (Teams.Cache.TryGetValue(x.TeamId, out TeamData team))
                                team.CachedMembers.Add(x.Id, x);
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
                        x.GetWebSocket();
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
                                    rcon.StartComms();
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

            Logger.LogMessage("Database", "Data Loaded", LogSeverity.Info);
            IsCacheDone = true;
        }

        if (HasException)
            return false;

        return true;
    }

    public static IMongoDatabase Run = null!;

    public static ICacheCollection<TeamData> Teams = null!;

    public static ConcurrentDictionary<string, TeamData> TeamsVanityCache = new ConcurrentDictionary<string, TeamData>();
    public static ConcurrentDictionary<ObjectId, string> VanityUrlCache = new ConcurrentDictionary<ObjectId, string>();

    public static ICacheCollection<ServerData> Servers = null!;

    public static Dictionary<ObjectId, PartialUserData> Users = new Dictionary<ObjectId, PartialUserData>();

    public static ICacheCollection<ConsoleData> Consoles = null;

    public static ICacheCollection<ProjectData> Projects = null!;

    public static ICacheCollection<WebsiteData> Websites = null!;

    public static ICacheCollection<LogData> Logs = null!;

    public static ICacheCollection<TeamRoleData> Roles = null!;

    public static ICacheCollection<TeamMemberData> Members = null!;

    public static ICacheCollection<VanityUrlData> TeamVanityUrls = null!;

    public static ICacheCollection<EmailTemplateData> EmailTemplates = null!;

    public static ICollection<AuditLog> AuditLogs = null!;

    public static ICacheCollection<APIClient> API = null!;
}
