using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.API;

public class APIClient
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public ObjectId OwnerId { get; set; }
    public ObjectId? TeamId { get; set; }
    public bool IsDisabled { get; set; }
    public bool IsInstanceAdmin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool UseCustomPermissions { get; set; }
    public ObjectId? GeneratedBy { get; set; }
    public DateTime? GeneratedAt { get; set; }
    public PermissionsSet CustomPermissions { get; set; } = new PermissionsSet();
    public string? TokenHash { get; set; }

    public bool HasAccess(TeamMemberData member)
    {
        if (OwnerId == member.UserId || member.HasTeamPermission(TeamPermission.ManageAllAPIs))
            return true;
        return false;
    }

    public bool CanManage(TeamMemberData member)
    {
        if ((member.HasTeamPermission(TeamPermission.ManageOwnAPIs) && OwnerId == member.UserId) || member.HasTeamPermission(TeamPermission.ManageAllAPIs))
            return true;
        return false;
    }

    public async Task UpdateAsync(UpdateDefinition<APIClient> update, Action action)
    {
        FilterDefinition<APIClient> filter = Builders<APIClient>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.API.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();

    }
}
