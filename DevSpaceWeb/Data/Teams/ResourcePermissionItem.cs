using DevSpaceWeb.Data.Permissions;
using MongoDB.Bson;

namespace DevSpaceWeb.Data.Teams;

public class ResourcePermissionItem
{
    public ObjectId? ObjectId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public PermissionsSet? Permissions { get; set; }
}
