using DevSpaceWeb.Data.API;
using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
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

    public AuditLog(TeamMemberData member, AuditLogCategoryType category, AuditLogEventType evnt)
    {
        CreatedAt = DateTime.UtcNow;
        TeamId = member.TeamId;
        ActionUserId = member.UserId;
        CategoryType = category;
        EventType = evnt;
    }

    public AuditLog AddProperty(string name, string value)
    {
        Properties.Add(name, value);
        return this;
    }

    public AuditLog AddPropertyChange(string name, string oldValue, string newValue)
    {
        if (oldValue != newValue)
            Properties.Add(name, name == "Password" ? null : newValue);
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

    public AuditLog SetTarget(ServerData server)
    {
        TargetType = AuditLogTargetType.Server;
        TargetId = server.Id;
        TargetName = server.Name;
        return this;
    }

    public AuditLog SetTarget(ConsoleData console)
    {
        TargetType = AuditLogTargetType.Console;
        TargetId = console.Id;
        TargetName = console.Name;
        return this;
    }

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

    //public AuditLog SetTarget(AuditLogTargetType type, ObjectId? target)
    //{
    //    TargetType = type;
    //    TargetId = target;
    //    return this;
    //}

    public AuditLog SetPermissions(PermissionsSet perms)
    {
        Permissions = perms;
        return this;
    }

    public AuditLog SetSensitive()
    {
        IsSensitive = true;
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
    public bool IsSensitive;
    public PermissionsSet? Permissions;

}
public enum AuditLogTargetType
{
    Instance, Team, Role, Member, Server, Website, Project, Log, API, Console
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
    RoleSettingsChanged, ServerOwnershipChanged, APIClientOwnershipChanged, ConsoleOwnershipChanged
}
