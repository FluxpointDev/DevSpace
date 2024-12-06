using DevSpaceWeb.Data.Permissions;
using MongoDB.Bson;

namespace DevSpaceWeb.Data;

public class AuditLog
{
    public AuditLog(ObjectId user, ObjectId? team, AuditLogCategoryType category, AuditLogEventType evnt)
    {
        CreatedAt = DateTime.UtcNow;
        TeamId = team;
        ActionUserId = user;
        CategoryType = category;
        EventType = evnt;
    }

    public AuditLog SetProperties(Dictionary<string, string> props)
    {
        Properties = props;
        return this;
    }

    public AuditLog SetTarget(AuditLogTargetType type, ObjectId? target)
    {
        TargetType = type;
        TargetId = target;
        return this;
    }

    public AuditLog SetPermissions(PermissionsSet perms)
    {
        Permissions = perms;
        return this;
    }

    public ObjectId Id;
    public DateTime CreatedAt;
    public ObjectId? TeamId;
    public ObjectId? ActionUserId;

    public AuditLogEventType EventType;
    public AuditLogCategoryType CategoryType;
    public AuditLogTargetType TargetType;
    public ObjectId? TargetId;
    public string TargetName;
    public Dictionary<string, string> Properties = new Dictionary<string, string>();
    public PermissionsSet? Permissions;
    
}
public enum AuditLogTargetType
{
    Instance, Team, Role, User, Server, Website, Project, Log
}
public enum AuditLogCategoryType
{
    Setting, Role, Permission, Member
}
public enum AuditLogEventType
{
    RoleCreated, RoleDeleted, SettingsChanged, IconChanged, IconRemoved, MemberAdded, MemberRemoved, MemberRolesChanged, OwnershipTransfered, DefaultPermissionsChanged, RolePermissionsChanged
}
