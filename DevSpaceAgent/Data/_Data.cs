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
            }
            catch
            {
                Console.WriteLine("Failed to create Data folder");
                return false;
            }
        }

        try
        {
            File.WriteAllText(Program.CurrentDirectory + "Data/WriteTest.txt", "Hi");
        }
        catch
        {
            Console.WriteLine("Failed to write in Data folder");
            return false;
        }

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




        if (!File.Exists(Program.CurrentDirectory + "Data/Cert.pfx") || string.IsNullOrEmpty(Config.CertKey))
        {
            Console.WriteLine("Create cert");
            Config.CertKey = GetRandomString(new Random().Next(26, 34)) + Guid.NewGuid().ToString().Replace("-", "");
            try
            {
                ECDsa ecdsa = ECDsa.Create(); // generate asymmetric key pair
                CertificateRequest req = new CertificateRequest("cn=devspace", ecdsa, HashAlgorithmName.SHA256);
                Program.Certificate = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));

                // Create PFX (PKCS #12) with private key
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
        }

        if (string.IsNullOrEmpty(Config.AgentKey))
        {
            Config.AgentKey = GetRandomString(new Random().Next(26, 34)) + Guid.NewGuid().ToString().Replace("-", "");
            SaveConfig = true;
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
        char[] characterArray = characterSet.Distinct().ToArray();
        if (characterArray.Length == 0)
            throw new ArgumentException("characterSet must not be empty", "characterSet");

        byte[] bytes = new byte[length * 8];
        new RNGCryptoServiceProvider().GetBytes(bytes);
        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
            ulong value = BitConverter.ToUInt64(bytes, i * 8);
            result[i] = characterArray[value % (uint)characterArray.Length];
        }
        return new string(result);
    }
}
