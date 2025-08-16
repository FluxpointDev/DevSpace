using Newtonsoft.Json;

namespace DevSpaceWeb.Database;

public class ConfigDatabase
{
    [JsonIgnore]
    public string? Name => Database.Name;

    public InlineDatabase Database = new InlineDatabase();

    public string GetConnectionString()
    {
        if (!string.IsNullOrEmpty(Database.ConnectionString))
            return Database.ConnectionString;

        if (string.IsNullOrEmpty(Database.User) || string.IsNullOrEmpty(Database.Password))
            return $"mongodb://{Database.Host}:{Database.Port}";

        return $"mongodb://{Database.User}:{Database.Password}@{Database.Host}:{Database.Port}";
    }
}
public class InlineDatabase
{
    public string? Host = "cloudfrost-mongodb";
    public short Port = 5557;
    public string? Name = "cloudfrost-dev";
    public string? User = "root";
    public string? Password = null;
    /// <summary>
    /// priority
    /// </summary>
    public string? ConnectionString;
}