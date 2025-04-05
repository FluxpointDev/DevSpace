namespace DevSpaceWeb.Data.Permissions;

[Flags]
public enum ConsolePermission : ulong
{
    /// <summary> Allows you to view this resource </summary>
    ViewConsole = 1L << 0,

    /// <summary> View permissions for this resource </summary>
    ViewPermissions = 1L << 1,

    /// <summary> Manage permissions for this resource </summary>
    ManagePermissions = 1L << 2,

    /// <summary> Manage all console settings for the team.s </summary>
    ManageConsole = 1L << 3,

    /// <summary> Create and manage all console resources in the team. </summary>
    ConsoleAdministrator = 1L << 4,

    /// <summary> Create this resource </summary>
    CreateConsoleResource = 1L << 5,

    /// <summary> Delete this resource </summary>
    DeleteConsoleResource = 1L << 6,

    /// <summary> Send console commands </summary>
    UseConsoleCommands = 1L << 7,

    /// <summary> View players list </summary>
    ViewPlayers = 1L << 8,

    /// <summary> Allows you to kick players </summary>
    KickPlayers = 1L << 9,

    /// <summary> Allows you to ban players </summary>
    BanPlayers = 1L << 10,

    /// <summary> View bans list </summary>
    ViewBans = 1L << 11,

    /// <summary> Manage the server with shutdown, lock, restart, etc </summary>
    ControlServer = 1L << 12,

    /// <summary> View console logs </summary>
    ViewConsoleLogs = 1L << 13,

    /// <summary> Lets you send messages to players </summary>
    MessagePlayers = 1L << 14,

    /// <summary> Lets you send global messages </summary>
    MessageGlobal = 1L << 15,

    /// <summary> Lets you manage the console connection settings </summary>
    ManageConnection = 1L << 16,

    /// <summary> Lets you view IPS </summary>
    ViewIPs = 1L << 17,

    /// <summary> Lets you view Authorized rcon connections </summary>
    ViewConnections = 1L << 18
}
