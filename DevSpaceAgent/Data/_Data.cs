using DevSpaceShared.Data;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace DevSpaceAgent.Data;

public static class _Data
{
    public static Config Config = null!;

    public static bool LoadConfig()
    {
        bool SaveConfig = false;

        if (!Directory.Exists(Program.CurrentDirectory + "Data"))
        {
            try
            {
                Directory.CreateDirectory(Program.CurrentDirectory + "Data");
                Console.WriteLine("[Config] Created Data folder");
            }
            catch
            {
                Console.WriteLine("[Config] Failed to create Data folder");
                return false;
            }
        }

        try
        {
            File.WriteAllText(Program.CurrentDirectory + "Data/WriteTest.txt", "Hi");
        }
        catch
        {
            Console.WriteLine("[Config] Failed to write in Data folder");
            return false;
        }

        if (!Directory.Exists(Program.CurrentDirectory + "Data/Temp"))
        {
            try
            {
                Directory.CreateDirectory(Program.CurrentDirectory + "Data/Temp");
                Console.WriteLine("[Config] Created Temp folder");
            }
            catch
            {
                Console.WriteLine("[Config] Failed to create Data/Temp folder");
                return false;
            }
        }

        if (!Directory.Exists(Program.CurrentDirectory + "Data/Stacks"))
            Directory.CreateDirectory(Program.CurrentDirectory + "Data/Stacks");

        if (File.Exists(Program.CurrentDirectory + "Data/Stacks.json"))
        {
            Dictionary<string, StackFile>? stacks = null;
            try
            {
                using (StreamReader reader = new StreamReader(Program.CurrentDirectory + "Data/Stacks.json"))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    stacks = (Dictionary<string, StackFile>?)serializer.Deserialize(reader, typeof(Dictionary<string, StackFile>));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to parse Stacks.json file, " + ex.Message);
            }

            if (stacks != null)
                Program.Stacks = stacks;
            else
                throw new Exception("Failed to parse Stacks.json file");

            Console.WriteLine("[Config] Loaded Stacks.json");
        }

        if (!Directory.Exists(Program.CurrentDirectory + "Data/Templates"))
        {
            try
            {
                Directory.CreateDirectory(Program.CurrentDirectory + "Data/Templates");
                Console.WriteLine("[Config] Created Templates folder");
            }
            catch
            {
                Console.WriteLine("[Config] Failed to create Data/Templates folder");
                return false;
            }
        }

        if (!File.Exists(Program.CurrentDirectory + "Data/Templates.json"))
        {
            Program.SaveTemplates();
            Console.WriteLine("[Config] Created Templates.json");
        }
        else
        {
            Dictionary<string, DockerCustomTemplate>? templates = null;
            try
            {
                using (StreamReader reader = new StreamReader(Program.CurrentDirectory + "Data/Templates.json"))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    templates = (Dictionary<string, DockerCustomTemplate>?)serializer.Deserialize(reader, typeof(Dictionary<string, DockerCustomTemplate>));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to parse Templates.json file, " + ex.Message);
            }

            if (templates != null)
                Program.CustomTemplates = templates;
            else
                throw new Exception("Failed to parse Templates.json file");

            Console.WriteLine("[Config] Loaded Templates.json");
        }

        if (!File.Exists(Program.CurrentDirectory + "Data/Config.json"))
        {
            Config = new Config();
            Console.WriteLine("[Config] Created Config.json");
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
                    config = (Config?)serializer.Deserialize(reader, typeof(Config));
                }
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

            Console.WriteLine("[Config] Loaded Config.json");
        }




        if (!File.Exists(Program.CurrentDirectory + "Data/Cert.pfx") || string.IsNullOrEmpty(Config.CertKey))
        {
            Console.WriteLine("[Config] Created Certificate");
            Config.CertKey = GetRandomString(new Random().Next(26, 34)) + Guid.NewGuid().ToString().Replace("-", "");
            try
            {
                ECDsa ecdsa = ECDsa.Create();
                CertificateRequest req = new CertificateRequest("cn=devspace", ecdsa, HashAlgorithmName.SHA256);
                Program.Certificate = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.UtcNow.AddYears(5));

                File.WriteAllBytes(Program.CurrentDirectory + "Data/Cert.pfx", Program.Certificate.Export(X509ContentType.Pfx, Config.CertKey));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            SaveConfig = true;
        }
        else
        {
            Program.Certificate = new X509Certificate2(Program.CurrentDirectory + "Data/Cert.pfx", Config.CertKey, X509KeyStorageFlags.PersistKeySet);
            Console.WriteLine("[Config] Loaded Certificate");
        }

        if (Program.Certificate == null)
            throw new Exception("Failed to load certificate.");

        if (string.IsNullOrEmpty(Config.AgentId))
        {
            Config.AgentId = Guid.NewGuid().ToString();
            SaveConfig = true;
        }

        if (string.IsNullOrEmpty(Config.AgentKey))
        {
            Config.AgentKey = GetRandomString(new Random().Next(26, 34)) + Guid.NewGuid().ToString().Replace("-", "");
            SaveConfig = true;
            Console.WriteLine("[Config] Generated Agent Key");
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
        "!#$&'()*+,/:;=?@[]";

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
