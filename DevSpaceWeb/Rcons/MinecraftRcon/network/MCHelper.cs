using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using LibMCRcon.WorldData;
using System.Threading.Tasks;




//!Classes directly related to the minecraft server.
namespace LibMCRcon.RCon
{
    //!Helper functions for various minecraft console commands
    public class MCHelper
    {
        bool isbusy = false;
        /// <summary>
        /// Without knowing the safe heighth to be teleported to, a search is done.
        /// Several command exchanges will occur with the server as fast as they can
        /// which may require throttle controlls.  If a safe space cannot be found, the player
        /// is returned from the location found prior to the transfer.  Any operators on the server will
        /// receive RCON spam.
        /// </summary>
        /// <param name="r">The active RCon connection.</param>
        /// <param name="sb">Output cache from RCon interaction, html encoded.</param>
        /// <param name="player">Player name on the server, must be logged in.</param>
        /// <param name="x">X axis</param>
        /// <param name="y">Y axis</param>
        /// <param name="z">Z axis</param>
        public void TeleportSafeSearch(TCPRcon r, StringBuilder sb, String player, Int32 x, Int32 y, Int32 z)
        {
            if (isbusy == true)
            {
                sb.AppendFormat(@"<br/>{0} Teleporter busy, try again in a bit...", DateTime.Now);
                return;
            }
            else
                isbusy = true;

            if (r.IsReadyForCommands)
            {
                try
                {

                    string resp = "";
                    bool safeTp = false;

                    r.ExecuteCmd("tell {0} To lands unknown you go!!!", player);
                    r.ExecuteCmd("gamemode sp {0}", player);
                    r.ExecuteCmd("tp {3} {0} {1} {2}", x, y, z, player);

                    sb.AppendFormat(@"<br/>{0} Initiated TP sequence for {1}. Waiting for chunk loads", DateTime.Now, player);
                    TimeCheck tchk = new TimeCheck(8000);

                    while (tchk.Expired == false)
                    {
                        resp = r.ExecuteCmd(@"testforblock {0} {1} {2} minecraft:air", x, y, z);
                        if (!resp.Contains("Cannot test"))
                            break;

                        Thread.Sleep(1000);
                    }

                    if (!resp.Contains("Successfully")
                        && r.ExecuteCmd("testforblock {0} {1} {2} minecraft:air", x, y + 1, z).Contains("Successfully")
                        && !r.ExecuteCmd("testforblock {0} {1} {2} minecraft:air", x, y - 1, z).Contains("Successfully")
                        && !r.ExecuteCmd("testforblock {0} {1} {2} minecraft:lava", x, y - 1, z).Contains("Successfully"))
                    {

                        sb.AppendFormat(@"<br/>{0} Found safe landing at {1} {3} {2}", DateTime.Now, x, z, y);
                        safeTp = true;
                    }

                    else
                    {


                        sb.AppendFormat(@"<br/>{0} Scanning for safe landing", DateTime.Now);

                        r.ExecuteCmd("tell {0} Looking for safe tp landing site...", player);


                        for (Int32 yy = 255; yy > 1; yy = yy - 15)
                        {
                            if (!r.ExecuteCmd("testforblock {0} {1} {2} minecraft:air", x, yy, z).Contains("Successfully") || yy < 30)
                                for (Int32 ys = yy + 15; ys > 1; ys--)
                                {


                                    if (!r.ExecuteCmd("testforblock {0} {1} {2} minecraft:air", x, ys, z).Contains("Successfully")
                                        && !r.ExecuteCmd("testforblock {0} {1} {2} minecraft:lava", x, ys, z).Contains("Successfully"))
                                    {

                                        if (r.ExecuteCmd("testforblock {0} {1} {2} minecraft:air", x, ys + 1, z).Contains("Successfully")
                                               && r.ExecuteCmd("testforblock {0} {1} {2} minecraft:air", x, ys + 2, z).Contains("Successfully"))
                                        {


                                            r.ExecuteCmd("tp {3} {0} {1} {2}", x, ys + 1, z, player);
                                            sb.AppendFormat(@"<br/>{0} Found safe landing at {1} {3} {2}", DateTime.Now, x, z, ys + 1);
                                            safeTp = true;
                                            break;
                                        }
                                    }


                                }

                            if (safeTp) break;
                        }

                    }

                    if (safeTp == false)
                    {

                        sb.AppendFormat(@"<br/>{0} No safe landing found, return to nexus", DateTime.Now);
                        r.ExecuteCmd("tell {0} No safe tp found, returned to nexus", player);
                        r.ExecuteCmd("tp {0} 0 65 0", player);
                        r.ExecuteCmd("gamemode s {0}", player);
                    }
                    else
                    {


                        r.ExecuteCmd("tell {0} Welcome to your new adventure...", player);
                        r.ExecuteCmd("gamemode s {0}", player);
                    }


                }
                catch (Exception)
                {
                    sb.AppendFormat(@"<br/>{0} Unexpected error occured in processing safe teleport", DateTime.Now);
                }
            }

            isbusy = false;
        }
        /// <summary>
        /// Locate the player and return a Voxel describing the location of the player in the world.
        /// This is achieved by attempting to teleport the player in place, which causes the server to 
        /// report back where the player is located in the success message.
        /// </summary>
        /// <param name="r">Active RCon object.</param>
        /// <param name="sb">Output cache from RCon execution, raw.</param>
        /// <param name="player">Player name on the server, must be logged in.</param>
        /// <returns>A Voxel, an object with X,Y,Z coordinates buddled together.</returns>

        public Voxel PlayerLocation(TCPRcon r, StringBuilder sb, String player)
        {
            Voxel pV = MinecraftOrdinates.Region();
            string result;
            if (r.IsReadyForCommands)
            {
                result = r.ExecuteCmd(string.Format("execute {0} ~ ~ ~ testforblock ~ ~-1 ~ minecraft:lava", player));
                if (result.Length > 0)
                {
                    int ix = result.IndexOf("is");
                    if (ix != -1)
                    {
                        string data = result.Substring(12, result.IndexOf("is") - 12);
                        string[] tpdata = data.Split(new char[] { ',' });

                        float tf = 0;

                        if (float.TryParse(tpdata[0], out tf))
                        {
                            pV.WorldX = (int)tf;
                            tf = 0;
                            if (float.TryParse(tpdata[1], out tf))
                            {
                                pV.WorldY = (int)tf;
                                tf = 0;
                                if (float.TryParse(tpdata[2], out tf))
                                {
                                    pV.WorldZ = (int)tf;
                                    return pV;
                                }

                            }
                        }
                    }
                }
            }

            pV.IsValid = false;
            return pV;

        }
        public Voxel PlayerLocationTP(TCPRcon r, StringBuilder sb, String player)
        {

            Voxel pV = new Voxel();
            string result;
            if (r.IsReadyForCommands)
            {
                try
                {

                    result = r.ExecuteCmd(string.Format("tp {0} ~ ~ ~", player));

                    sb.AppendFormat("{0}: {1}", player, result);

                    //!Filter output, strip out response language and break out X,Y,Z coordinates
                    string[] data = result.Split(new string[] { "to" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] tpdata = data[1].Split(',');

                    try
                    {
                        pV.WorldX = (int)float.Parse(tpdata[0]);
                        pV.WorldY = (int)float.Parse(tpdata[1]);
                        pV.WorldZ = (int)float.Parse(tpdata[2]);
                    }
                    catch (Exception)
                    {
                        sb.AppendLine("Error in parsing player x,y,z");
                        pV.IsValid = false;
                    }

                }
                catch (Exception e)
                {
                    sb.AppendFormat("Error in locating player:{0}", e.Message);
                    pV.IsValid = false;
                }


            }
            else
                pV.IsValid = false;

            return pV;

        }

        public static TCPRcon ActivateRcon(string Host, int port, string password)
        {
            var r = new TCPRcon(Host, port, password);
            r.StartComms();
            return r;

        }
        public static async Task<TCPRconAsync> ActivateRconAsync(string Host, int port, string password)
        {
            var r = new TCPRconAsync(Host, port, password);
            await r.StartComms();
            return r;

        }
        public static List<string> LoadPlayers(TCPRcon r, StringBuilder sb)
        {

            string[] list_players;


            if (r.IsReadyForCommands == true)
            {

                try
                {
                    r.Clear();

                    string resp = r.ExecuteCmd("list");

                    string[] list_cmd = resp.Split(':');
                    list_players = list_cmd[1].Replace(" ", string.Empty).Split(',');

                }
                catch (Exception ee)
                {
                    list_players = new string[] {};
                    sb.AppendFormat(@"{0} => Connection:{1}, Network:{2}", ee.Message, r.LastTCPError, r.StateTCP, r.StateRCon);

                }


            }
            else
            {
                list_players = new string[] { "RCON_ERROR" };
            }


            return new List<string>(list_players);

        }


    }

}
