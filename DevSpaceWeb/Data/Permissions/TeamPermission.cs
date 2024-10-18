namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum TeamPermission : ulong
{
    ViewMembers = 1L << 0,
    ManageMembers = 1L << 1,
    ManageRoles = 1L << 2,
    ManagePermissions = 1L << 3,
    ManageTeam = 1L << 4,
    TeamAdministrator = 1L << 5,
    GlobalAdministrator = 1L << 6,
}
