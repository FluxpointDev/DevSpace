namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum ServerPermission : ulong
{
    /// <summary>
    /// View the server.
    /// </summary>
    ViewServer = 1L << 0,

    /// <summary>
    /// View permissions for the server.
    /// </summary>
    ViewPermissions = 1L << 1,

    /// <summary>
    /// Change permissions for the server.
    /// </summary>
    ManagePermissions = 1L << 2,

    /// <summary>
    /// Change server settings.
    /// </summary>
    ManageServer = 1L << 3,

    /// <summary>
    /// Create and manage all server resources in the team.
    /// </summary>
    ServerAdministrator = 1L << 4,

    /// <summary>
    /// Create and own server resources for the team.
    /// </summary>
    CreateServerResource = 1L << 5,

    /// <summary>
    /// Delete server resource from the team.
    /// </summary>
    DeleteServerResource = 1L << 6,

    /// <summary>
    /// View or update the server agent connection settings.
    /// </summary>
    ManageConnection = 1L << 7,

    /// <summary>
    /// View server host information and IP.
    /// </summary>
    ViewHostInfo = 1L << 8,
}
