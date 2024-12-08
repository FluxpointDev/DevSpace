namespace DevSpaceWeb.Data.Users;

public class UserSession
{
    public bool IsMobile;
    public SessionBrowserType BrowserType;
    public string Country;
    public string Name;
    public HashSet<string> AuthorizedIps = new HashSet<string>();
    public DateTime CreatedAt = DateTime.UtcNow;
}
public enum SessionBrowserType
{
    Unknown, InternetExplorer, Firefox, Chrome, Safari, Edge, Opera, Vivaldi
}
