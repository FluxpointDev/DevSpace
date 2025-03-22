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
        if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password))
            return $"mongodb://{Host}:{Port}";
        return $"mongodb://{User}:{Password}@{Host}:{Port}";
    }
}