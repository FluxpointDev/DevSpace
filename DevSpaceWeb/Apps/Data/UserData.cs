namespace DevSpaceWeb.Apps.Data;

public class UserData
{
    public UserSettings Settings = new UserSettings();
    public UserWorkspaceSettings WorkspaceSettings = new UserWorkspaceSettings();
}

public class UserSettings
{
    public UserLanguageType Language;
}
public class UserWorkspaceSettings
{
    public bool MinimapEnabled = true;
    public bool GridDotsEnabled = true;
    public bool GridSnapEnabled = true;
    public bool HighlightEnabled = false;
}

public enum UserLanguageType
{
    English
}