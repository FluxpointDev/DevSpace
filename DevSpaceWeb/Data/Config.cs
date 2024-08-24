using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson;
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
        using (StreamWriter file = File.CreateText(Program.Directory.Data.Path + $"Config.json"))
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
    public string Email;
    public bool HasIcon;
    public int IconVersion;
    public string PublicUrl;
    public ConfigLimits Limits = new ConfigLimits();
    public ConfigFeatures Features = new ConfigFeatures();

    public string GetIconOrDefault(bool usePng = false)
    {
        if (!HasIcon)
            return "https://cdn.fluxpoint.dev/devspace/instance_icon." + (usePng ? "png" : "webp");

        return "";
    }
}
public class ConfigFeatures
{
    public bool APIEnabled = true;
    public bool AllowUnauthenticatedPublicFolderAccess { get; set; }
    public bool SwaggerEnabled = true;
    public bool SwaggerUIEnabled = true;
}
public class ConfigLimits
{
    public int MaxImageWidthHeight = 3000;
    public int MaxImageSizeMegabytes = 10;
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

    public void RemoveActiveEmailTemplate(EmailTemplateType type)
    {
        ActiveEmailTemplates.Remove(type);
        _Data.Config.Save();
    }

    public void SetActiveEmailTemplate(EmailTemplateType type, EmailTemplateData data)
    {
        if (ActiveEmailTemplates.ContainsKey(type))
            ActiveEmailTemplates.Remove(type);

        ActiveEmailTemplates.Add(type, data.Id);
        _Data.Config.Save();
    }

    public string GetActiveEmailTemplateName(EmailTemplateType type)
    {
        if (ActiveEmailTemplates.TryGetValue(type, out var tempId) && _DB.EmailTemplates.Cache.TryGetValue(tempId.ToString(), out var template))
            return template.Name;

        return "Default " + new EmailTemplateData { Type = type }.GetTypeName();
    }

    public Dictionary<EmailTemplateType, ObjectId> ActiveEmailTemplates = new Dictionary<EmailTemplateType, ObjectId>();

    public EmailTemplateData GetActiveTemplateOrDefault(EmailTemplateType type)
    {
        if (ActiveEmailTemplates.TryGetValue(type, out ObjectId id) && _DB.EmailTemplates.Cache.TryGetValue(id.ToString(), out var template) && !string.IsNullOrEmpty(template.Body) && !template.IsDisabled)
            return template;

        return new EmailTemplateData { Body = EmailTemplateDefaults.List[type], Type = type };
    }
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