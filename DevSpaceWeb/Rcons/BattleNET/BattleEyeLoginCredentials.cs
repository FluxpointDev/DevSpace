/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.3.4 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2018 by it's authors.                    *
 *  Some rights reserved. See license.txt, authors.txt.    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Net;

namespace BattleNET
{
    public struct BattlEyeLoginCredentials
    {
        public BattlEyeLoginCredentials(IPAddress host, int port, string password)
            : this()
        {
            Host = host;
            Port = port;
            Password = password;
        }

        public IPAddress Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
    }
}