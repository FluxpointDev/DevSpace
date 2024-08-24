using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Websites;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace DevSpaceWeb.Database;

public static class _DB
{
    public static MongoClient Client = null!;

    public static bool IsConnected;
    private static bool IsCacheDone;

    public static event SessionEventHandler SessionUpdated;

    public static void TriggerSessionEvent(ObjectId user, SessionEventType type)
    {
        SessionUpdated?.Invoke(null, user, type);
    }

    public static async Task<bool> StartAsync()
    {
        IsConnected = false;
        if (Run == null)
        {
            Run = Client.GetDatabase(_Data.Config.Database.Name);
            Teams = new ICollection<TeamData>("teams");
            Users = new ICollection<UserData>("users");
            Servers = new ICollection<ServerData>("servers");
            Projects = new ICollection<ProjectData>("projects");
            Websites = new ICollection<WebsiteData>("websites");
            Logs = new ICollection<LogData>("logs");
            EmailTemplates = new ICollection<EmailTemplateData>("email_templates");
            TeamVanityUrls = new ICollection<VanityUrlData>("vanity_urls");
        }

        try
        {

            var result = await Run.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
            IsConnected = true;
            Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
        }
        catch (Exception ex) { Console.WriteLine(ex); return false; }

        if (!IsCacheDone)
        {
            await Teams.Find(Builders<TeamData>.Filter.Empty).ForEachAsync(x =>
            {
                Teams.Cache.TryAdd(x.Id.ToString(), x);

                if (!string.IsNullOrEmpty(x.VanityUrl))
                {
                    TeamsVanityCache.TryAdd(x.VanityUrl, x);
                    VanityUrlCache.TryAdd(x.Id, x.VanityUrl);
                }
            });

            _ = TeamVanityUrls.Find(Builders<VanityUrlData>.Filter.Empty).ForEachAsync(x =>
            {
                if (TeamVanityUrls.Cache.TryAdd(x.Id.ToString(), x))
                {
                    foreach (var i in x.ServerVanityUrls)
                    {
                        VanityUrlCache.TryAdd(i.Value, i.Key);
                    }
                    foreach (var i in x.LogsVanityUrls)
                    {
                        VanityUrlCache.TryAdd(i.Value, i.Key);
                    }
                    foreach (var i in x.ProjectVanityUrls)
                    {
                        VanityUrlCache.TryAdd(i.Value, i.Key);
                    }
                    foreach (var i in x.WebsiteVanityUrls)
                    {
                        VanityUrlCache.TryAdd(i.Value, i.Key);
                    }
                }

            });

            _ = EmailTemplates.Find(Builders<EmailTemplateData>.Filter.Empty).ForEachAsync(x =>
            {
                EmailTemplates.Cache.TryAdd(x.Id.ToString(), x);
            });

            _ = Logs.Find(Builders<LogData>.Filter.Empty).ForEachAsync(x =>
            {
                Logs.Cache.TryAdd(x.Id.ToString(), x);
            });

            _ = Servers.Find(Builders<ServerData>.Filter.Empty).ForEachAsync(x =>
            {
                Servers.Cache.TryAdd(x.Id.ToString(), x);
                x.GetWebSocket();
            });

            _ = Websites.Find(Builders<WebsiteData>.Filter.Empty).ForEachAsync(x =>
            {
                Websites.Cache.TryAdd(x.Id.ToString(), x);
            });

            _ = Projects.Find(Builders<ProjectData>.Filter.Empty).ForEachAsync(x =>
            {
                Projects.Cache.TryAdd(x.Id.ToString(), x);
            });
            IsCacheDone = true;
        }

        return true;
    }

    public static IMongoDatabase Run = null!;

    public static ICollection<TeamData> Teams = null!;

    public static ConcurrentDictionary<string, TeamData> TeamsVanityCache = new ConcurrentDictionary<string, TeamData>();
    public static ConcurrentDictionary<ObjectId, string> VanityUrlCache = new ConcurrentDictionary<ObjectId, string>();

    public static ICollection<ServerData> Servers = null!;

    public static ICollection<UserData> Users = null!;

    public static ICollection<ProjectData> Projects = null!;

    public static ICollection<WebsiteData> Websites = null!;

    public static ICollection<LogData> Logs = null!;

    public static ICollection<VanityUrlData> TeamVanityUrls = null!;

    public static ICollection<EmailTemplateData> EmailTemplates = null!;
}
