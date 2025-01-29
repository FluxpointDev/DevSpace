namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum APIPermission : ulong
{
    APIAdministrator = 1L << 0,
    ViewOwnAPIs = 1L << 1,
    ManageOwnAPIs = 1L << 2,
    ViewAllAPIs = 1L << 3
}
