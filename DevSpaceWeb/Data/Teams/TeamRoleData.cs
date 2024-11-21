using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Database;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Teams;

public class TeamRoleData
{
    [BsonId]
    public ObjectId Id { get; set; }
    public ObjectId TeamId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public PermissionsSet Permissions { get; set; } = new PermissionsSet();

    [JsonIgnore]
    [BsonIgnore]
    private PermissionsAll _Permissions => new PermissionsAll(Permissions);

    //public bool HasTeamPermission(TeamMemberData member, TeamPermission permission)
    //{
    //    if (member.Team == null)
    //        return false;

    //    if (_Permissions.Team.GlobalAdministrator)
    //        return true;

    //    return _Permissions.Team.Has(permission);
    //}

    //public bool HasLogPermission(TeamMemberData member, LogPermission permission)
    //{
    //    if (member.Team == null)
    //        return false;

    //    if (_Permissions.Team.GlobalAdministrator || _Permissions.Log.LogAdministrator)
    //        return true;

    //    return _Permissions.Log.Has(permission);
    //}

    //public bool HasProjectPermission(TeamMemberData member, ProjectPermission permission)
    //{
    //    if (member.Team == null)
    //        return false;

    //    if (_Permissions.Team.GlobalAdministrator || _Permissions.Project.ProjectAdministrator)
    //        return true;

    //    return _Permissions.Project.Has(permission);
    //}

    //public bool HasServerPermission(TeamMemberData member, ServerPermission permission)
    //{
    //    if (member.Team == null)
    //        return false;

    //    if (_Permissions.Team.GlobalAdministrator || _Permissions.Server.ServerAdministrator)
    //        return true;

    //    return _Permissions.Server.Has(permission);
    //}

    //public bool HasWebsitePermission(TeamMemberData member, WebsitePermission permission)
    //{
    //    if (member.Team == null)
    //        return false;

    //    if (_Permissions.Team.GlobalAdministrator || _Permissions.Website.WebsiteAdministrator)
    //        return true;

    //    return _Permissions.Website.Has(permission);
    //}

    //public bool HasDockerPermission(TeamMemberData member, DockerPermission permission)
    //{
    //    if (member.Team == null)
    //        return false;

    //    if (_Permissions.Team.GlobalAdministrator || _Permissions.Docker.DockerAdministrator)
    //        return true;

    //    return _Permissions.Docker.Has(permission);
    //}

    public async Task UpdateAsync(UpdateDefinition<TeamRoleData> update)
    {
        FilterDefinition<TeamRoleData> filter = Builders<TeamRoleData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Roles.Collection.UpdateOneAsync(filter, update);
        if (!Result.IsAcknowledged)
            throw new InvalidOperationException("Failed to update team data");
    }
}