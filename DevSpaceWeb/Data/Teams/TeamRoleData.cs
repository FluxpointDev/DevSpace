using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DevSpaceWeb.Data.Teams;

public class TeamRoleData : IObject
{
    public ObjectId TeamId { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public string? Color { get; set; }

    [Obsolete]
    [BsonIgnoreIfNull]
    public int? Position { get; set; }

    public PermissionsSet Permissions { get; set; } = new PermissionsSet();

    [JsonIgnore]
    [BsonIgnore]
    public TeamData? Team => _DB.Teams.Cache.GetValueOrDefault(TeamId);

    public int GetPosition()
    {
        if (Team != null && Team.RolePositions.TryGetValue(Id, out int position))
            return position;

        return 0;
    }

    public bool CanManage(TeamMemberData? currentMember)
    {
        if (currentMember == null || TeamId != currentMember.TeamId)
            return false;

        if (Team != null && Team.OwnerId == currentMember.UserId)
            return true;

        if (currentMember.GetRank() > GetPosition())
            return true;
        return false;
    }

    public async Task UpdateAsync(UpdateDefinition<TeamRoleData> update, Action action)
    {
        FilterDefinition<TeamRoleData> filter = Builders<TeamRoleData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Roles.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }
}