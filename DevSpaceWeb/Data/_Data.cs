using DaRT;
using DevSpaceWeb.Agents;
using DevSpaceWeb.Data.Proxmox;
using Discord.Rest;
using LibMCRcon.RCon;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Security.Cryptography;
using static DevSpaceWeb.Apps.WorkspaceController;

namespace DevSpaceWeb.Data;

public static class _Data
{
    public static Config Config = null!;
    public static Dictionary<ObjectId, RCon> BattleyeRcons = [];
    public static Dictionary<ObjectId, TCPRconAsync> MinecraftRcons = [];
    public static Dictionary<ObjectId, ProxmoxAgent> ProxmoxAgents = [];
    public static Dictionary<ObjectId, EdgeAgent> EdgeAgents = [];
    public static Dictionary<ObjectId, DiscordRestClient> DiscordClients = [];
    public static WorkspaceConfig WorkspaceData = null!;

    public static bool LoadConfig()
    {
        bool SaveConfig = false;

        if (Program.Directory == null)
        {
            try
            {
                Program.Directory = new DirectoryStructureMain(Program.IsDevMode ? AppDomain.CurrentDomain.BaseDirectory : "/Data/");
            }
            catch
            {
                Logger.LogMessage($"Failed to write data to the program path: {Program.Directory?.Path}", LogSeverity.Error);
                Environment.Exit(1);
            }

        }

        if (!File.Exists(Program.Directory.Data.Path + "Config.json"))
        {
            Config = new Config();
            SaveConfig = true;
        }
        else
        {
            Config? config = null;
            try
            {
                using (StreamReader reader = new StreamReader(Program.Directory.Data.Path + "Config.json"))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    config = (Config?)serializer.Deserialize(reader, typeof(Config));
                }
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Failed to parse Config.json file, " + ex.Message, LogSeverity.Error);
            }

            try
            {
                using (StreamReader reader = new StreamReader("Toolbox.json"))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    WorkspaceData = (WorkspaceConfig?)serializer.Deserialize(reader, typeof(WorkspaceConfig));
                }
            }
            catch
            {
                Logger.LogMessage("Failed to load Toolbox.json", LogSeverity.Error);
            }

            if (config == null)
            {
                Logger.LogMessage("Failed to load Config.json file.", LogSeverity.Error);
                return false;
            }

            Config = config;
        }

        if (Config.Admin == null)
        {
            SaveConfig = true;
            Config.Admin = new ConfigAdmin();
        }

        if (SaveConfig)
            Config.Save();

        return true;
    }

    public static string GetRandomString(int length)
    {
        const string characterSet =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        "abcdefghijklmnopqrstuvwxyz" +
        "0123456789" +
        "#$*+,/:?@[]";

        if (length < 0)
            throw new ArgumentException("length must not be negative", "length");

        if (length > int.MaxValue / 8)
            throw new ArgumentException("length is too big", "length");

        if (characterSet == null)
            throw new ArgumentNullException("characterSet");

        char[] characterArray = characterSet.Distinct().ToArray();
        if (characterArray.Length == 0)
            throw new ArgumentException("characterSet must not be empty", "characterSet");

        byte[] bytes = new byte[length * 8];
#pragma warning disable SYSLIB0023 // Type or member is obsolete
        new RNGCryptoServiceProvider().GetBytes(bytes);
#pragma warning restore SYSLIB0023 // Type or member is obsolete
        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
            ulong value = BitConverter.ToUInt64(bytes, i * 8);
            result[i] = characterArray[value % (uint)characterArray.Length];
        }
        return new string(result);
    }
}
