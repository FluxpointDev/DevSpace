namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum TeamPermission : ulong
{
    /// <summary>
    /// View all members in the team.
    /// </summary>
    ViewMembers = 1L << 0,

    /// <summary>
    /// Add, remove or manage members for the team.
    /// </summary>
    ManageMembers = 1L << 1,

    /// <summary>
    /// Give or remove roles to members.
    /// </summary>
    AssignRoles = 1L << 2,

    /// <summary>
    /// Create, delete and manage roles.
    /// </summary>
    ManageRoles = 1L << 3,

    /// <summary>
    /// View role and member permissions.
    /// </summary>
    ViewPermissions = 1L << 4,

    /// <summary>
    /// Change permissions for roles except global admin.
    /// </summary>
    ManagePermissions = 1L << 5,

    /// <summary>
    /// Change team settings.
    /// </summary>
    ManageTeam = 1L << 6,

    /// <summary>
    /// All team permissions.
    /// </summary>
    TeamAdministrator = 1L << 7,

    /// <summary>
    /// All permissions for the team and resources.
    /// </summary>
    GlobalAdministrator = 1L << 8,
    UNUSED_ = 1L << 9,

    /// <summary>
    /// View changes to the team and resources.
    /// </summary>
    ViewAuditLogs = 1L << 10,

    UNUSED1_ = 1L << 11,
    UNUSED2_ = 1L << 12,
    UNUSED3_ = 1L << 13,

    /// <summary>
    /// 
    /// </summary>
    ViewRoles = 1L << 14,
    UNUSED4_ = 1L << 15
}
