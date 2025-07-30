using DevSpaceWeb.Data.API;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using MongoDB.Bson;

namespace DevSpaceWeb.Data;

public class AuditLog : IObject
{
    public AuditLog(ObjectId user, ObjectId? team, AuditLogCategoryType category, AuditLogEventType evnt)
    {
        CreatedAt = DateTime.UtcNow;
        TeamId = team;
        ActionUserId = user;
        CategoryType = category;
        EventType = evnt;
    }

    public AuditLog(TeamMemberData member, AuditLogCategoryType category, AuditLogEventType evnt)
    {
        CreatedAt = DateTime.UtcNow;
        TeamId = member.TeamId;
        ActionUserId = member.UserId;
        CategoryType = category;
        EventType = evnt;
    }

    public AuditLog AddProperty(string name, string? value)
    {
        Properties.Add(name, value);
        return this;
    }

    public AuditLog AddProperty(string name, bool value)
    {
        Properties.Add(name, value.ToString());
        return this;
    }

    public AuditLog AddProperty(string name, int value)
    {
        Properties.Add(name, value.ToString());
        return this;
    }

    public AuditLog AddPropertyChange(string name, string? oldValue, string? newValue)
    {
        if (oldValue != newValue)
            Properties.Add(name, (name == "Password") ? null : newValue);
        return this;
    }

    public AuditLog AddPropertyChange(string name, bool oldValue, bool newValue)
    {
        if (oldValue != newValue)
            Properties.Add(name, newValue.ToString());
        return this;
    }

    public AuditLog AddPropertyChange(string name, int oldValue, int newValue)
    {
        if (oldValue != newValue)
            Properties.Add(name, newValue.ToString());
        return this;
    }

    public AuditLog SetTarget(TeamData team)
    {
        TargetType = AuditLogTargetType.Team;
        TargetId = team.Id;
        return this;
    }

    public AuditLog SetTarget(APIClient client)
    {
        TargetType = AuditLogTargetType.API;
        TargetId = client.Id;
        TargetName = client.Name;
        return this;
    }

    public AuditLog SetTarget(ITeamResource app)
    {
        TargetType = GetResourceType(app.ResourceType);
        TargetId = app.Id;
        TargetName = app.Name;
        return this;
    }

    private AuditLogTargetType GetResourceType(ResourceType type) => type switch
    {
        ResourceType.App => AuditLogTargetType.App,
        ResourceType.Console => AuditLogTargetType.Console,
        ResourceType.Link => AuditLogTargetType.Link,
        ResourceType.Project => AuditLogTargetType.Project,
        ResourceType.Server => AuditLogTargetType.Server,
        ResourceType.Website => AuditLogTargetType.Website,
        _ => throw new NotImplementedException(),
    };

    public AuditLog SetTarget(TeamRoleData role)
    {
        TargetType = AuditLogTargetType.Role;
        TargetId = role.Id;
        TargetName = role.Name;
        return this;
    }

    public AuditLog SetTarget(TeamMemberData member)
    {
        TargetType = AuditLogTargetType.Member;
        TargetId = member.Id;
        TargetName = _DB.Users[member.UserId].UserName;
        return this;
    }

    public AuditLog SetTarget(PartialUserData user)
    {
        TargetType = AuditLogTargetType.Member;
        TargetId = user.Id;
        TargetName = user.UserName;
        return this;
    }

    public AuditLog SetTarget(AuthUser user)
    {
        TargetType = AuditLogTargetType.Member;
        TargetId = user.Id;
        TargetName = user.UserName;
        return this;
    }

    public AuditLog SetPermissions(PermissionsSet perms)
    {
        Permissions = perms;
        return this;
    }
    public AuditLog SetFlags(AuditLogFlag flags)
    {
        Flags = flags;
        return this;
    }

    public AuditLog SetSensitive()
    {
        Flags = AuditLogFlag.Sensitive;
        return this;
    }

    public DateTime CreatedAt;
    public ObjectId? TeamId;
    public ObjectId? ActionUserId;

    public AuditLogEventType EventType;
    public AuditLogCategoryType CategoryType;
    public AuditLogTargetType TargetType;
    public ObjectId? TargetId;
    public string? TargetName;
    public Dictionary<string, string?> Properties = [];
    [Obsolete]
    public bool IsSensitive;
    public PermissionsSet? Permissions;
    public AuditLogFlag Flags;
}

public enum AuditLogTargetType
{
    Instance, Team, Role, Member, Server, Website, Project, Link, API, Console, App
}
public enum AuditLogCategoryType
{
    Setting, Role, Permission, Member, Team, Resource
}
public enum AuditLogEventType
{
    RoleCreated, RoleDeleted,
    TeamSettingsChanged,
    IconChanged, IconRemoved,
    MemberAdded, MemberRemoved, MemberRolesChanged,
    TeamOwnershipChanged, DefaultPermissionsChanged,
    RolePermissionsChanged,
    APIClientCreated, APIClientDeleted, APIClientPermissionsChanged, APIClientTokenGenerated, APIClientEnabled, APIClientDisabled, APIClientSettingsChanged,
    ServerCreated, ServerDeleted, ServerPermissionsChanged, ServerSettingsChanged,
    ConsoleCreated, ConsoleDeleted, ConsolePermissionsChanged, ConsoleSettingsChanged, ConsoleRconChanged,
    RoleSettingsChanged, ServerOwnershipChanged, APIClientOwnershipChanged, ConsoleOwnershipChanged, RolePositionChanged,
    MemberNicknameChanged, ServerOnboard,
    AppCreated, AppDeleted, AppPermissionsChanged, AppOwnershipChanged, AppSettingsChanged,
    WebsiteCreated, WebsiteDeleted, WebsitePermissionsChanged, WebsiteOwnershipChanged, WebsiteSettingsChanged,
    ProjectCreated, ProjectDeleted, ProjectPermissionsChanged, ProjectOwnershipChanged, ProjectSettingsChanged,
    LinkCreated, LinkDeleted,
    StatusMonitorCreated, StatusMonitorDeleted, StatusMonitorPermissionsChanged, StatusMonitorSettingsChanged,
    StatusPageCreated, StatusPageDeleted, StatusPagePermissionsChanged, StatusPageSettingsChanged,
}
[Flags]
public enum AuditLogFlag : ulong
{
    Automatic = 1L << 0,
    Sensitive = 1L << 1,
}