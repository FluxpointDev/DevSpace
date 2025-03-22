/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.3.4 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2018 by it's authors.                    *
 *  Some rights reserved. See license.txt, authors.txt.    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using DaRT;

namespace BattleNET;

public delegate void BattlEyeMessageEventHandler(BattlEyeMessageEventArgs args);

public class BattlEyeMessageEventArgs : EventArgs
{
    public BattlEyeMessageEventArgs(string message, int id)
    {
        Message = message;
        Id = id;
        TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, cstZone);

        var formattedTime = Convert.ToDateTime((cstTime.ToShortDateString() + " " + cstTime.Hour + ":" + cstTime.Minute));

        Time = (formattedTime.ToString() + " ").Replace(":00 ", " ");
    }

    public string Message { get; }
    public string FilteredMessage { get; set; }

    public string Time;

    public LogType Type { get; set; } = LogType.Debug;
    public int Id { get; }
}
