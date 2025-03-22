/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.3.4 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2018 by it's authors.                    *
 *  Some rights reserved. See license.txt, authors.txt.    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.ComponentModel;

namespace BattleNET;

public enum BattlEyeCommand
{
    /// <summary>
    /// #init - Reload server config file loaded by –config option.
    /// </summary>
    [Description("#init")]
    Init,

    /// <summary>
    /// #shutdown - Shuts down the server.
    /// </summary>
    [Description("#shutdown")]
    Shutdown,

    /// <summary>
    /// #reassign - Start over and reassign roles.
    /// </summary>
    [Description("#reassign")]
    Reassign,

    /// <summary>
    /// #restart - Restart mission.
    /// </summary>
    [Description("#restart")]
    Restart,

    /// <summary>
    /// #restartserver - Restart server.
    /// </summary>
    [Description("#restartserver")]
    RestartServer,

    /// <summary>
    /// #lock - Locks the server, prevents new clients from joining.
    /// </summary>
    [Description("#lock")]
    Lock,

    /// <summary>
    /// #unlock - Unlocks the server, allows new clients to join.
    /// </summary>
    [Description("#unlock")]
    Unlock,

    /// <summary>
    /// #mission [missionName] - Loads the given mission on the server.
    /// </summary>
    [Description("#mission ")]
    Mission,

    /// <summary>
    /// missions - Returns a list of the available missions on the server.
    /// </summary>
    [Description("missions")]
    Missions,

    /// <summary>
    /// RConPassword [password] - Changes the RCon password.
    /// </summary>
    [Description("RConPassword ")]
    RConPassword,

    /// <summary>
    /// MaxPing [ping] - Changes the MaxPing value. If a player has a higher ping, he will be kicked from the server.
    /// </summary>
    [Description("MaxPing ")]
    MaxPing,

    /// <summary>
    /// kick [player#] - Kicks a player. His # can be found in the player list using the 'players' command.
    /// </summary>
    [Description("kick ")]
    Kick,

    /// <summary>
    /// players - Displays a list of the players on the server including BE GUIDs and pings.
    /// </summary>
    [Description("players")]
    Players,

    /// <summary>
    /// Say [player#] [msg] - Say something to player #. specially -1 equals all players on server (e.g. 'Say -1 Hello World').
    /// </summary>
    [Description("Say ")]
    Say,

    /// <summary>
    /// loadBans - (Re)load the BE ban list from bans.txt.
    /// </summary>
    [Description("loadBans")]
    LoadBans,

    /// <summary>
    /// loadScripts - Loads the scripts.txt file without the need to restart server.
    /// </summary>
    [Description("loadScripts")]
    LoadScripts,

    /// <summary>
    /// loadEvents - (Re)load createvehicle.txt, remoteexec.txt and publicvariable.txt
    /// </summary>
    [Description("loadEvents")]
    LoadEvents,

    /// <summary>
    /// bans - Show a list of all BE server bans.
    /// </summary>
    [Description("bans")]
    Bans,

    /// <summary>
    /// ban [player #] [time in minutes] [reason] - Ban a player's BE GUID from the server. If time is not specified or 0, the ban will be permanent; if reason is not specified the player will be kicked with "Banned".
    /// </summary>
    [Description("ban ")]
    Ban,

    /// <summary>
    /// addBan [GUID] [time in minutes] [reason] - Same as "ban", but allows to ban a player that is not currently on the server.
    /// </summary>
    [Description("addBan ")]
    AddBan,

    /// <summary>
    /// removeBan [ban #] - Remove ban (get the ban # from the bans command).
    /// </summary>
    [Description("removeBan ")]
    RemoveBan,

    /// <summary>
    /// writeBans - Removes expired bans from bans file.
    /// </summary>
    [Description("writeBans")]
    WriteBans,

    /// <summary>
    /// admins - Gets connected RCON clients.
    /// </summary>
    [Description("admins")]
    Admins,
}