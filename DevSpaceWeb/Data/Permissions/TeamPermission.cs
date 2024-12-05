namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum TeamPermission : ulong
{
    ViewMembers = 1L << 0,
    ManageMembers = 1L << 1,
    ManageRoles = 1L << 2,
    ViewPermissions = 1L << 3,
    ManagePermissions = 1L << 4,
    ManageTeam = 1L << 5,
    TeamAdministrator = 1L << 6,
    GlobalAdministrator = 1L << 7,
    ManageResources = 1L << 8,
}
