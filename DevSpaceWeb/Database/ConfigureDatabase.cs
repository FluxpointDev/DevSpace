namespace DevSpaceWeb.Database;

public class ConfigDatabase
{
    public string? Host = "devspace-mongodb";
    public short Port = 5557;
    public string? Name = "devspace";
    public string? User = "root";
    public string? Password = "";
    /// <summary>
    /// priority
    /// </summary>
    public string? ConnectionString;

    public string GetConnectionString()
    {
        if (!string.IsNullOrEmpty(ConnectionString))
            return ConnectionString;

        if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password))
            return $"mongodb://{Host}:{Port}";

        return $"mongodb://{User}:{Password}@{Host}:{Port}";
    }
}