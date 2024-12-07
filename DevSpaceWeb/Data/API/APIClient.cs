using DevSpaceWeb.Data.Permissions;
using MongoDB.Bson;

namespace DevSpaceWeb.Data.API;

public class APIClient
{
    public ObjectId Id { get; set; }
    public ObjectId OwnerId { get; set; }
    public ObjectId AccessUserId { get; set; }
    public ObjectId? TeamId { get; set; }
    public bool IsDisabled { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool UseCustomPermissions { get; set; }
    public PermissionsSet CustomPermissions { get; set; } = new PermissionsSet();
}
