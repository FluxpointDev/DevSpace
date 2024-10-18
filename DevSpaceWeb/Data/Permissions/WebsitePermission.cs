namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum WebsitePermission : ulong
{
    ViewWebsite = 1L << 0,
    ManagePermissions = 1L << 1,
    WebsiteAdministrator = 1L << 2,
}
