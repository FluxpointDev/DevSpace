namespace DevSpaceWeb.Data;

public static class SearchData
{
    public static List<SearchDataItem> List = new List<SearchDataItem>
    {
        new SearchDataItem("Change Username", "account", "/user/account"),
        new SearchDataItem("Change Display Name", "account", "/user/account"),
        new SearchDataItem("Change Email", "account", "/user/account"),
        new SearchDataItem("Delete Account", "account", "/user/account"),
        new SearchDataItem("Disable Account", "account", "/user/account"),
        new SearchDataItem("Change Password", "account", "/user/account"),
        new SearchDataItem("Change Background", "account", "/user/account"),
        new SearchDataItem("Setup 2FA", "account", "/user/account"),
        new SearchDataItem("Setup Two-factor Authentication", "account", "/user/account"),
        new SearchDataItem("Logins", "account", "/user/account"),
        new SearchDataItem("Sessions", "account", "/user/account"),
        new SearchDataItem("Connections", "account", "/user/account"),
        new SearchDataItem("Passkeys", "account", "/user/account"),
        new SearchDataItem("WebAuthn", "account", "/user/account"),

    };
}
public class SearchDataItem
{
    public SearchDataItem(string name, string type, string path)
    {
        Name = name;
        Type = type;
        Path = path;
    }
    public string Name;
    public string Type;
    public string Path;
    public string[] Tags = new string[0];
}