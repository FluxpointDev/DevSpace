using MongoDB.Bson;

namespace DevSpaceWeb.Data;

public class AuditLog
{
    public AuditLog(ObjectId? team, AuthUser user, AuditLogCategoryType category, AuditLogEventType evnt)
    {
        CreatedAt = DateTime.UtcNow;
        TeamId = team;
        ActionUserId = user.Id;
        CategoryType = category;
        EventType = evnt;
    }

    public void SetTarget(AuditLogTargetType type, ObjectId target)
    {
        TargetType = type;
        TargetId = target;
    }

    public ObjectId Id;
    public DateTime CreatedAt;
    public ObjectId? TeamId;
    public ObjectId? ActionUserId;

    public AuditLogEventType EventType;
    public AuditLogCategoryType CategoryType;
    public AuditLogTargetType TargetType;
    public ObjectId? TargetId;
    
}
public enum AuditLogTargetType
{
    Instance, Team, Role, Member, Server, Docker, Website, Project, Log
}
public enum AuditLogCategoryType
{
    Settings, Roles, Permissions
}
public enum AuditLogEventType
{
    RoleCreated, RoleDeleted
}
