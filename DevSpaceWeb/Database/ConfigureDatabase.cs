namespace DevSpaceWeb.Database;

public class ConfigureDatabase
{
    public string? Name;
    public string? Host;
    public short Port;
    public string? User;
    public string? Password;

    public string GetConnectionString()
    {
        return $"mongodb://{User}:{Password}@{Host}:{Port}";
    }
}