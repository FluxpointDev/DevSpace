using Newtonsoft.Json;
using System.Security.Cryptography;

namespace DevSpaceWeb.Data;

public static class _Data
{
    public static Config Config = null!;

    public static bool LoadConfig()
    {
        bool SaveConfig = false;

        if (!Directory.Exists(Program.CurrentDirectory + "Data"))
            Directory.CreateDirectory(Program.CurrentDirectory + "Data");

        if (!Directory.Exists(Program.CurrentDirectory + "Public"))
            Directory.CreateDirectory(Program.CurrentDirectory + "Public");

        if (!Directory.Exists(Program.CurrentDirectory + "Data/Cache"))
            Directory.CreateDirectory(Program.CurrentDirectory + "Data/Cache");

        if (!File.Exists(Program.CurrentDirectory + "Data/Config.json"))
        {
            Config = new Config();
            SaveConfig = true;
        }
        else
        {
            Config? config = null;
            try
            {
                using (StreamReader reader = new StreamReader(Program.CurrentDirectory + "Data/Config.json"))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    config = (Config)serializer.Deserialize(reader, typeof(Config));
                }
                config.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to parse Config.json file, " + ex.Message);
            }

            if (config == null)
            {
                Console.WriteLine("Failed to load Config.json file.");
                return false;
            }

            Config = config;
        }

        if (string.IsNullOrEmpty(Config.AdminKey))
        {
            SaveConfig = true;
            Config.AdminKey = GetRandomString(new Random().Next(26, 34)) + Guid.NewGuid().ToString().Replace("-", "");
        }

        if (Config.Database == null)
        {
            SaveConfig = true;
            Config.Database = new ConfigDatabase();
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
        "0123456789";

        if (length < 0)
            throw new ArgumentException("length must not be negative", "length");
        if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
            throw new ArgumentException("length is too big", "length");
        if (characterSet == null)
            throw new ArgumentNullException("characterSet");
        var characterArray = characterSet.Distinct().ToArray();
        if (characterArray.Length == 0)
            throw new ArgumentException("characterSet must not be empty", "characterSet");

        var bytes = new byte[length * 8];
        new RNGCryptoServiceProvider().GetBytes(bytes);
        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            ulong value = BitConverter.ToUInt64(bytes, i * 8);
            result[i] = characterArray[value % (uint)characterArray.Length];
        }
        return new string(result);
    }
}
