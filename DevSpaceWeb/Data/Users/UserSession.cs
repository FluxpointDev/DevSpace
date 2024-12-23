namespace DevSpaceWeb.Data.Users;

public class UserSession
{
    public bool IsMobile;
    public SessionBrowserType BrowserType;
    public string Country;
    public string Name;
    public HashSet<string> AuthorizedIps = new HashSet<string>();
    public DateTime CreatedAt = DateTime.UtcNow;
    public DateTime? LastLoginAt;

    public static UserSession Create(UserSessionJson json)
    {
        return new UserSession
        {
            Name = Utils.GetBrowserName((SessionBrowserType)json.BrowserType),
            Country = json.Country,
            BrowserType = (SessionBrowserType)json.BrowserType,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            IsMobile = json.IsMobile
        };
    }
}
public class UserSessionJson
{
    public bool IsMobile { get; set; }
    public int BrowserType { get; set; }
    public string Country { get; set; }
    public string Email { get; set; }
}
public enum SessionBrowserType
{
    Unknown, InternetExplorer, Firefox, Chrome, Safari, Edge, Opera, Vivaldi
}
