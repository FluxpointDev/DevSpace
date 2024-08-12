using Newtonsoft.Json;

namespace DevSpaceWeb;

public class Config
{
    public bool isFullySetup = false;
    public string AdminKey;
    public ConfigDatabase Database = new ConfigDatabase();
    public ConfigAdmin Admin = new ConfigAdmin();
    public ConfigAuth Auth = new ConfigAuth();
    public ConfigInstance Instance = new ConfigInstance();
    public ConfigEmail Email = new ConfigEmail();
    public ConfigProviders Providers = new ConfigProviders();

    public void Save()
    {
        using (StreamWriter file = File.CreateText(Program.CurrentDirectory + $"Data/Config.json"))
        {
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            serializer.Serialize(file, this);
        }
    }
}
public class ConfigInstance
{
    public string Name = "Dev Space Self-hosted";
    public string Description;
    public string Icon;
    public string PublicUrl;
    public ConfigLimits Limits = new ConfigLimits();
    public ConfigFeatures Features = new ConfigFeatures();
}
public class ConfigFeatures
{
    public bool APIEnabled = true;
    public bool SwaggerEnabled = true;
    public bool SwaggerUIEnabled = true;
}
public class ConfigLimits
{
    public int MaxImagesUpload = 100;
}
public class ConfigAdmin
{

}
public class ConfigAuth
{
    public bool AllowInternalLogin = true;
    public bool AllowRegister = false;
    public bool AllowGoogleAuth = false;
    public bool AllowFluxpointAuth = false;
}
public class ConfigDatabase
{
    public bool IsSetup;
    public string Name = "devspace";
    public string IP = "127.0.0.1";
    public int Port = 27017;
    public string Username;
    public string Password;
    public string RootUsername = "devspace_root";
    public string RootPassword;

    public string GetConnectionString()
    {
        return $"mongodb://{Username}:{Password}@{IP}:{Port}";
    }
}
public class ConfigEmail
{
    public string SmtpHost;
    public int SmtpPort;
    public string SmtpUser;
    public string SmtpPassword;
    public ConfigEmailType Type;
    public string ManagedEmailToken;
}
public enum ConfigEmailType
{
    FluxpointManaged, Custom
}
public class ConfigProviders
{
    public IConfigProvider Google = new IConfigProvider();
    public IConfigProvider GitHub = new IConfigProvider();
    public IConfigProvider Discord = new IConfigProvider();
}
public class IConfigProvider
{
    public string ClientId;
    public string ClientSecret;
}