/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.3.4 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2018 by it's authors.                    *
 *  Some rights reserved. See license.txt, authors.txt.    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.ComponentModel;

namespace BattleNET
{
    public enum BattlEyeConnectionResult
    {
        [Description("Connected!")]
        Success,

        [Description("Host unreachable!")]
        ConnectionFailed,

        [Description("Invalid login details!")]
        InvalidLogin
    }
}
