using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Websites;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Teams;

public class IResource
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public ObjectId OwnerId { get; set; }
}
public class ITeamResource : IResource
{
    public ITeamResource(ResourceType type)
    {
        ResourceType = type;
    }

    public ResourceType ResourceType;

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, PermissionsSet> MemberPermissionOverrides = new Dictionary<ObjectId, PermissionsSet>();

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, PermissionsSet> RolePermissionOverrides = new Dictionary<ObjectId, PermissionsSet>();

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public Dictionary<ObjectId, PermissionsSet> ApiPermissionOverrides = new Dictionary<ObjectId, PermissionsSet>();

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
            Dictionary<ObjectId, PermissionsSet> Permissions = new Dictionary<ObjectId, PermissionsSet>();
            Permissions.Add(roleId, perms);
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
                case ResourceType.Console:
                    {
                        FilterDefinition<ConsoleData> filter = Builders<ConsoleData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ConsoleData> update = new UpdateDefinitionBuilder<ConsoleData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Consoles.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Log:
                    {
                        FilterDefinition<LogData> filter = Builders<LogData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<LogData> update = new UpdateDefinitionBuilder<LogData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Logs.Collection.UpdateOne(filter, update);
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
            Dictionary<ObjectId, PermissionsSet> Permissions = new Dictionary<ObjectId, PermissionsSet>();
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
                case ResourceType.Console:
                    {
                        FilterDefinition<ConsoleData> filter = Builders<ConsoleData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<ConsoleData> update = new UpdateDefinitionBuilder<ConsoleData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Consoles.Collection.UpdateOne(filter, update);
                    }
                    break;
                case ResourceType.Log:
                    {
                        FilterDefinition<LogData> filter = Builders<LogData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<LogData> update = new UpdateDefinitionBuilder<LogData>()
                            .Set(x => x.RolePermissionOverrides, Permissions);

                        Result = _DB.Logs.Collection.UpdateOne(filter, update);
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
            Dictionary<ObjectId, PermissionsSet> Permissions = new Dictionary<ObjectId, PermissionsSet>();
            Permissions.Add(memberId, perms);
            foreach (KeyValuePair<ObjectId, PermissionsSet> i in MemberPermissionOverrides)
            {
                if (memberId != i.Key)
                    Permissions.Add(i.Key, i.Value);
            }

            UpdateResult? Result = null;
            switch (ResourceType)
            {
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
                case ResourceType.Log:
                    {
                        FilterDefinition<LogData> filter = Builders<LogData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<LogData> update = new UpdateDefinitionBuilder<LogData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.Logs.Collection.UpdateOne(filter, update);
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
            Dictionary<ObjectId, PermissionsSet> Permissions = new Dictionary<ObjectId, PermissionsSet>();
            foreach (KeyValuePair<ObjectId, PermissionsSet> i in MemberPermissionOverrides)
            {
                if (memberId != i.Key)
                    Permissions.Add(i.Key, i.Value);
            }

            UpdateResult? Result = null;
            switch (ResourceType)
            {
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
                case ResourceType.Log:
                    {
                        FilterDefinition<LogData> filter = Builders<LogData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<LogData> update = new UpdateDefinitionBuilder<LogData>()
                            .Set(x => x.MemberPermissionOverrides, Permissions);

                        Result = _DB.Logs.Collection.UpdateOne(filter, update);
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
            Dictionary<ObjectId, PermissionsSet> Permissions = new Dictionary<ObjectId, PermissionsSet>();
            Permissions.Add(apiId, perms);
            foreach (KeyValuePair<ObjectId, PermissionsSet> i in ApiPermissionOverrides)
            {
                if (apiId != i.Key)
                    Permissions.Add(i.Key, i.Value);
            }

            UpdateResult? Result = null;
            switch (ResourceType)
            {
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
                case ResourceType.Log:
                    {
                        FilterDefinition<LogData> filter = Builders<LogData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<LogData> update = new UpdateDefinitionBuilder<LogData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.Logs.Collection.UpdateOne(filter, update);
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
            Dictionary<ObjectId, PermissionsSet> Permissions = new Dictionary<ObjectId, PermissionsSet>();
            foreach (KeyValuePair<ObjectId, PermissionsSet> i in ApiPermissionOverrides)
            {
                if (apiId != i.Key)
                    Permissions.Add(i.Key, i.Value);
            }

            UpdateResult? Result = null;
            switch (ResourceType)
            {
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
                case ResourceType.Log:
                    {
                        FilterDefinition<LogData> filter = Builders<LogData>.Filter.Eq(r => r.Id, Id);
                        UpdateDefinition<LogData> update = new UpdateDefinitionBuilder<LogData>()
                            .Set(x => x.ApiPermissionOverrides, Permissions);

                        Result = _DB.Logs.Collection.UpdateOne(filter, update);
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
            }

            if (Result != null && Result.IsAcknowledged)
                ApiPermissionOverrides.Remove(apiId);
        }
    }
}
public enum ResourceType
{
    Server, Console, Project, Log, Website,
}