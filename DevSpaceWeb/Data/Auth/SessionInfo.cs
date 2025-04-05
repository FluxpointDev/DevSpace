using DevSpaceWeb.Data.Users;

namespace DevSpaceWeb.Data.Auth;

public class SessionInfo
{
    public string? Email;
    public string? Country { get; set; }
    public SessionBrowserType Browser { get; set; }
    public bool IsMobile { get; set; }
}
