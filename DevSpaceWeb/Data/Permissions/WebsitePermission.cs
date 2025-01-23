namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum WebsitePermission : ulong
{
    ViewWebsite = 1L << 0,
    ViewPermissions = 1L << 1,
    ManagePermissions = 1L << 2,
    ManageWebsite = 1L << 3,
    WebsiteAdministrator = 1L << 4,
    ManageResource = 1L << 5,
}
