using MongoDB.Bson;

namespace DevSpaceWeb.Data;

public class AuditLog
{
    public AuditLog(AuthUser user, ObjectId? team, AuditLogCategoryType category, AuditLogEventType evnt)
    {
        CreatedAt = DateTime.UtcNow;
        TeamId = team;
        ActionUserId = user.Id;
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

    public ObjectId Id;
    public DateTime CreatedAt;
    public ObjectId? TeamId;
    public ObjectId? ActionUserId;

    public AuditLogEventType EventType;
    public AuditLogCategoryType CategoryType;
    public AuditLogTargetType TargetType;
    public ObjectId? TargetId;
    public Dictionary<string, string> Properties = new Dictionary<string, string>();
    
}
public enum AuditLogTargetType
{
    Instance, Team, Role, Member, Server, Docker, Website, Project, Log
}
public enum AuditLogCategoryType
{
    Setting, Role, Permission, Member
}
public enum AuditLogEventType
{
    RoleCreated, RoleDeleted
}
