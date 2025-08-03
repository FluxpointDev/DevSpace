using DevSpaceWeb.Apps.Data;
using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Status;
using DevSpaceWeb.Data.Websites;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Teams;

public class IBaseObject
{

}
public class IObject : IBaseObject
{
    [BsonId]
    public ObjectId Id { get; set; }
}
public class IResource : IObject
{
    public required string Name { get; set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public ObjectId OwnerId { get; set; }
}
public abstract class ITeamResource : IResource
{
    public ITeamResource(ResourceType type)
    {
        ResourceType = type;
    }

    public ResourceType ResourceType;

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, PermissionsSet> MemberPermissionOverrides = [];

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, PermissionsSet> RolePermissionOverrides = [];

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, PermissionsSet> ApiPermissionOverrides = [];

    public PermissionsSet DefaultPermissions { get; set; } = new PermissionsSet();

    public string? VanityUrl { get; set; }

    public string GetVanityUrlOrId()
    {
        if (!string.IsNullOrEmpty(VanityUrl))
            return VanityUrl;

        return Id.ToString();
    }

    public ObjectId TeamId { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId);

    [JsonIgnore]
    [BsonIgnore]
    public object RolePermissionsLock = new object();

    public void AddRolePermissionAsync(ObjectId roleId, PermissionsSet perms)
    {
        lock (RolePermissionsLock)
        {
            Dictionary<ObjectId, PermissionsSet> Permissions = new Dictionary<ObjectId, PermissionsSet>
            {
                { roleId, perms }
            };
            foreach (KeyValuePair<ObjectId, PermissionsSet> i in RolePermissionOverrides)
            {
                if (roleId != i.Key)
                    Permissions.Add(i.Key, i.Value);
            }

            UpdateResult? Result = null;
            switch (ResourceType)
            {
                case ResourceType.Server:
                    {
                        FilterDefinition<ServerData> filter = Builders<ServerData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ServerData> update = new UpdateDefinitionBuilder<ServerData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Servers.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.App:
                    {
                        FilterDefinition<AppData> filter = Builders<AppData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<AppData> update = new UpdateDefinitionBuilder<AppData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Apps.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Console:
                    {
                        FilterDefinition<ConsoleData> filter = Builders<ConsoleData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ConsoleData> update = new UpdateDefinitionBuilder<ConsoleData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Consoles.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Project:
                    {
                        FilterDefinition<ProjectData> filter = Builders<ProjectData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ProjectData> update = new UpdateDefinitionBuilder<ProjectData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Projects.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Website:
                    {
                        FilterDefinition<WebsiteData> filter = Builders<WebsiteData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<WebsiteData> update = new UpdateDefinitionBuilder<WebsiteData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Websites.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.StatusMonitor:
                    {
                        FilterDefinition<StatusMonitorData> filter = Builders<StatusMonitorData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<StatusMonitorData> update = new UpdateDefinitionBuilder<StatusMonitorData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.StatusMonitors.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.StatusPage:
                    {
                        FilterDefinition<StatusPageData> filter = Builders<StatusPageData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<StatusPageData> update = new UpdateDefinitionBuilder<StatusPageData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.StatusPages.Collection.UpdateOne(filter, update);
                    }
                    break;
            }

            if (Result != null && Result.IsAcknowledged)
            {
                if (!RolePermissionOverrides.TryAdd(roleId, perms))
                    RolePermissionOverrides[roleId] = perms;
            }
        }
    }

    public void DeleteRolePermissionAsync(ObjectId roleId)
    {
        lock (RolePermissionsLock)
        {
            Dictionary<ObjectId, PermissionsSet> Permissions = [];
            foreach (KeyValuePair<ObjectId, PermissionsSet> i in RolePermissionOverrides)
            {
                if (roleId != i.Key)
                    Permissions.Add(i.Key, i.Value);
            }

            UpdateResult? Result = null;
            switch (ResourceType)
            {
                case ResourceType.App:
                    {
                        FilterDefinition<AppData> filter = Builders<AppData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<AppData> update = new UpdateDefinitionBuilder<AppData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Apps.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Server:
                    {
                        FilterDefinition<ServerData> filter = Builders<ServerData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ServerData> update = new UpdateDefinitionBuilder<ServerData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Servers.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Console:
                    {
                        FilterDefinition<ConsoleData> filter = Builders<ConsoleData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ConsoleData> update = new UpdateDefinitionBuilder<ConsoleData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Consoles.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Project:
                    {
                        FilterDefinition<ProjectData> filter = Builders<ProjectData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ProjectData> update = new UpdateDefinitionBuilder<ProjectData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Projects.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Website:
                    {
                        FilterDefinition<WebsiteData> filter = Builders<WebsiteData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<WebsiteData> update = new UpdateDefinitionBuilder<WebsiteData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Websites.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.StatusMonitor:
                    {
                        FilterDefinition<StatusMonitorData> filter = Builders<StatusMonitorData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<StatusMonitorData> update = new UpdateDefinitionBuilder<StatusMonitorData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.StatusMonitors.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.StatusPage:
                    {
                        FilterDefinition<StatusPageData> filter = Builders<StatusPageData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<StatusPageData> update = new UpdateDefinitionBuilder<StatusPageData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.StatusPages.Collection.UpdateOne(filter, update);
                    }
                    break;
            }

            if (Result != null && Result.IsAcknowledged)
            {
                RolePermissionOverrides.Remove(roleId);
            }
        }
    }

    [JsonIgnore]
    [BsonIgnore]
    public object MemberPermissionsLock = new object();

    public void AddMemberPermissionAsync(ObjectId memberId, PermissionsSet perms)
    {
        lock (MemberPermissionsLock)
        {
            Dictionary<ObjectId, PermissionsSet> Permissions = new Dictionary<ObjectId, PermissionsSet>
            {
                { memberId, perms }
            };
            foreach (KeyValuePair<ObjectId, PermissionsSet> i in MemberPermissionOverrides)
            {
                if (memberId != i.Key)
                    Permissions.Add(i.Key, i.Value);
            }

            UpdateResult? Result = null;
            switch (ResourceType)
            {
                case ResourceType.App:
                    {
                        FilterDefinition<AppData> filter = Builders<AppData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<AppData> update = new UpdateDefinitionBuilder<AppData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.Apps.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Server:
                    {
                        FilterDefinition<ServerData> filter = Builders<ServerData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ServerData> update = new UpdateDefinitionBuilder<ServerData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.Servers.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Console:
                    {
                        FilterDefinition<ConsoleData> filter = Builders<ConsoleData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ConsoleData> update = new UpdateDefinitionBuilder<ConsoleData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.Consoles.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Project:
                    {
                        FilterDefinition<ProjectData> filter = Builders<ProjectData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ProjectData> update = new UpdateDefinitionBuilder<ProjectData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.Projects.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Website:
                    {
                        FilterDefinition<WebsiteData> filter = Builders<WebsiteData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<WebsiteData> update = new UpdateDefinitionBuilder<WebsiteData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.Websites.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.StatusMonitor:
                    {
                        FilterDefinition<StatusMonitorData> filter = Builders<StatusMonitorData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<StatusMonitorData> update = new UpdateDefinitionBuilder<StatusMonitorData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.StatusMonitors.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.StatusPage:
                    {
                        FilterDefinition<StatusPageData> filter = Builders<StatusPageData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<StatusPageData> update = new UpdateDefinitionBuilder<StatusPageData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.StatusPages.Collection.UpdateOne(filter, update);
                    }
                    break;

            }

            if (Result != null && Result.IsAcknowledged)
            {
                if (!MemberPermissionOverrides.TryAdd(memberId, perms))
                    MemberPermissionOverrides[memberId] = perms;
            }
        }
    }

    public void DeleteMemberPermissionAsync(ObjectId memberId)
    {
        lock (MemberPermissionsLock)
        {
            Dictionary<ObjectId, PermissionsSet> Permissions = [];
            foreach (KeyValuePair<ObjectId, PermissionsSet> i in MemberPermissionOverrides)
            {
                if (memberId != i.Key)
                    Permissions.Add(i.Key, i.Value);
            }

            UpdateResult? Result = null;
            switch (ResourceType)
            {
                case ResourceType.App:
                    {
                        FilterDefinition<AppData> filter = Builders<AppData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<AppData> update = new UpdateDefinitionBuilder<AppData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.Apps.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Server:
                    {
                        FilterDefinition<ServerData> filter = Builders<ServerData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ServerData> update = new UpdateDefinitionBuilder<ServerData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.Servers.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Console:
                    {
                        FilterDefinition<ConsoleData> filter = Builders<ConsoleData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ConsoleData> update = new UpdateDefinitionBuilder<ConsoleData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.Consoles.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Project:
                    {
                        FilterDefinition<ProjectData> filter = Builders<ProjectData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ProjectData> update = new UpdateDefinitionBuilder<ProjectData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.Projects.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Website:
                    {
                        FilterDefinition<WebsiteData> filter = Builders<WebsiteData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<WebsiteData> update = new UpdateDefinitionBuilder<WebsiteData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.Websites.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.StatusMonitor:
                    {
                        FilterDefinition<StatusMonitorData> filter = Builders<StatusMonitorData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<StatusMonitorData> update = new UpdateDefinitionBuilder<StatusMonitorData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.StatusMonitors.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.StatusPage:
                    {
                        FilterDefinition<StatusPageData> filter = Builders<StatusPageData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<StatusPageData> update = new UpdateDefinitionBuilder<StatusPageData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.StatusPages.Collection.UpdateOne(filter, update);
                    }
                    break;
            }

            if (Result != null && Result.IsAcknowledged)
                MemberPermissionOverrides.Remove(memberId);
        }
    }

    [JsonIgnore]
    [BsonIgnore]
    public object ApiPermissionsLock = new object();

    public void AddApiPermissionAsync(ObjectId apiId, PermissionsSet perms)
    {
        lock (MemberPermissionsLock)
        {
            Dictionary<ObjectId, PermissionsSet> Permissions = new Dictionary<ObjectId, PermissionsSet>
            {
                { apiId, perms }
            };
            foreach (KeyValuePair<ObjectId, PermissionsSet> i in ApiPermissionOverrides)
            {
                if (apiId != i.Key)
                    Permissions.Add(i.Key, i.Value);
            }

            UpdateResult? Result = null;
            switch (ResourceType)
            {
                case ResourceType.App:
                    {
                        FilterDefinition<AppData> filter = Builders<AppData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<AppData> update = new UpdateDefinitionBuilder<AppData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.Apps.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Server:
                    {
                        FilterDefinition<ServerData> filter = Builders<ServerData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ServerData> update = new UpdateDefinitionBuilder<ServerData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.Servers.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Console:
                    {
                        FilterDefinition<ConsoleData> filter = Builders<ConsoleData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ConsoleData> update = new UpdateDefinitionBuilder<ConsoleData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.Consoles.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Project:
                    {
                        FilterDefinition<ProjectData> filter = Builders<ProjectData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ProjectData> update = new UpdateDefinitionBuilder<ProjectData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.Projects.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Website:
                    {
                        FilterDefinition<WebsiteData> filter = Builders<WebsiteData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<WebsiteData> update = new UpdateDefinitionBuilder<WebsiteData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.Websites.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.StatusMonitor:
                    {
                        FilterDefinition<StatusMonitorData> filter = Builders<StatusMonitorData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<StatusMonitorData> update = new UpdateDefinitionBuilder<StatusMonitorData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.StatusMonitors.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.StatusPage:
                    {
                        FilterDefinition<StatusPageData> filter = Builders<StatusPageData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<StatusPageData> update = new UpdateDefinitionBuilder<StatusPageData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.StatusPages.Collection.UpdateOne(filter, update);
                    }
                    break;
            }

            if (Result != null && Result.IsAcknowledged)
            {
                if (!ApiPermissionOverrides.TryAdd(apiId, perms))
                    ApiPermissionOverrides[apiId] = perms;
            }
        }
    }

    public void DeleteApiPermissionAsync(ObjectId apiId)
    {
        lock (MemberPermissionsLock)
        {
            Dictionary<ObjectId, PermissionsSet> Permissions = [];
            foreach (KeyValuePair<ObjectId, PermissionsSet> i in ApiPermissionOverrides)
            {
                if (apiId != i.Key)
                    Permissions.Add(i.Key, i.Value);
            }

            UpdateResult? Result = null;
            switch (ResourceType)
            {
                case ResourceType.App:
                    {
                        FilterDefinition<AppData> filter = Builders<AppData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<AppData> update = new UpdateDefinitionBuilder<AppData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.Apps.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Server:
                    {
                        FilterDefinition<ServerData> filter = Builders<ServerData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ServerData> update = new UpdateDefinitionBuilder<ServerData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.Servers.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Console:
                    {
                        FilterDefinition<ConsoleData> filter = Builders<ConsoleData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ConsoleData> update = new UpdateDefinitionBuilder<ConsoleData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.Consoles.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Project:
                    {
                        FilterDefinition<ProjectData> filter = Builders<ProjectData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ProjectData> update = new UpdateDefinitionBuilder<ProjectData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.Projects.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Website:
                    {
                        FilterDefinition<WebsiteData> filter = Builders<WebsiteData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<WebsiteData> update = new UpdateDefinitionBuilder<WebsiteData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.Websites.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.StatusMonitor:
                    {
                        FilterDefinition<StatusMonitorData> filter = Builders<StatusMonitorData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<StatusMonitorData> update = new UpdateDefinitionBuilder<StatusMonitorData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.StatusMonitors.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.StatusPage:
                    {
                        FilterDefinition<StatusPageData> filter = Builders<StatusPageData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<StatusPageData> update = new UpdateDefinitionBuilder<StatusPageData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.StatusPages.Collection.UpdateOne(filter, update);
                    }
                    break;
            }

            if (Result != null && Result.IsAcknowledged)
                ApiPermissionOverrides.Remove(apiId);
        }
    }

    public abstract Task<bool> DeleteAsync(TeamMemberData? member, Action? action = null);
}
public enum ResourceType
{
    Server, Console, Project, Website, App, Link, StatusMonitor, StatusPage
}