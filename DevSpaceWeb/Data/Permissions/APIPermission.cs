namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum APIPermission : ulong
{
    /// <summary>
    /// Create and manage all API clients in the team.
    /// </summary>
    APIAdministrator = 1L << 0,

    /// <summary>
    /// View all API clients that you own.
    /// </summary>
    ViewOwnAPIs = 1L << 1,

    /// <summary>
    /// Manage all API clients that you own.
    /// </summary>
    ManageOwnAPIs = 1L << 2,

    /// <summary>
    /// View all API clients in the team.
    /// </summary>
    ViewAllAPIs = 1L << 3,

    /// <summary>
    /// Create and own api clients for the team.
    /// </summary>
    CreateOwnAPIs = 1L << 4
}
