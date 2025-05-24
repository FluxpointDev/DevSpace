namespace DevSpaceWeb.Data.Users;

public class UserSession
{
    public bool IsMobile { get; private set; }
    public SessionBrowserType BrowserType { get; private set; }
    public string? Country { get; private set; }
    public string? Name;
    public HashSet<string> AuthorizedIps { get; private set; } = [];
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt;

    public static UserSession Create(UserSessionJson json)
    {
        return new UserSession
        {
            Name = Utils.GetBrowserName((SessionBrowserType)json.BrowserType),
            Country = json.Country,
            BrowserType = (SessionBrowserType)json.BrowserType,
            LastLoginAt = DateTime.UtcNow,
            IsMobile = json.IsMobile
        };
    }
}
public class UserSessionJson
{
    public bool IsMobile { get; set; }
    public int BrowserType { get; set; }
    public string? Country { get; set; }
    public string? Email { get; set; }
}
public enum SessionBrowserType
{
    Unknown, InternetExplorer, Firefox, Chrome, Safari, Edge, Opera, Vivaldi
}
