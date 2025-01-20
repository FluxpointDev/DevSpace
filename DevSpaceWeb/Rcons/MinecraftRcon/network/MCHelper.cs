//!Classes directly related to the minecraft server.
namespace LibMCRcon.RCon
{
    //!Helper functions for various minecraft console commands
    public class MCHelper
    {
        public static List<string> LoadPlayers(TCPRcon r)
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
                    list_players = new string[] { };

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
