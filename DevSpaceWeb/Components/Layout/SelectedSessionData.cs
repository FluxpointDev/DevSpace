using DevSpaceWeb.Apps.Data;
using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Websites;

namespace DevSpaceWeb.Components.Layout;

public class SelectedSessionData
{
    public SelectedTeamData? Team { get; set; }

    public TeamMemberData? Member { get { return Team != null && Team.Member != null ? Team.Member : null; } }

    public AppData? App { get { return Team != null && Team.App != null ? Team.App.Data : null; } }

    public ServerData? Server { get { return Team != null && Team.Server != null ? Team.Server.Data : null; } }

    public WebsiteData? Website { get { return Team != null && Team.Website != null ? Team.Website.Data : null; } }

    public ProjectData? Project { get { return Team != null && Team.Project != null ? Team.Project.Data : null; } }

    public ConsoleData? Console { get { return Team != null && Team.Console != null ? Team.Console.Data : null; } }
}
public class SelectedTeamData
{
    public SelectedTeamData(TeamData team)
    {
        Id = team.Id.ToString();
        Data = team;

        VanityUrl = team.GetVanityUrlOrId();
    }

    public string Id { get; private set; }
    public TeamData Data { get; private set; }

    public string VanityUrl { get; set; }
    public TeamMemberData Member { get; set; }
    public SelectedAppData? App { get; set; }
    public SelectedServerData? Server { get; set; }
    public SelectedWebsiteData? Website { get; set; }
    public SelectedProjectData? Project { get; set; }
    public SelectedConsoleData? Console { get; set; }
}

public class SelectedConsoleData
{
    public SelectedConsoleData(ConsoleData console)
    {
        Id = console.Id.ToString();
        Data = console;

        VanityUrl = console.GetVanityUrlOrId();
    }

    public string Id { get; private set; }
    public ConsoleData Data { get; private set; }
    public string VanityUrl { get; set; }
    public bool IsVisible { get; set; } = true;
}
public class SelectedAppData
{
    public SelectedAppData(AppData app)
    {
        Id = app.Id.ToString();
        Data = app;

        VanityUrl = app.GetVanityUrlOrId();
    }

    public string Id { get; private set; }
    public AppData Data { get; private set; }
    public string VanityUrl { get; set; }
    public bool IsVisible { get; set; } = true;
}
public class SelectedServerData
{
    public SelectedServerData(ServerData server)
    {
        Id = server.Id.ToString();
        Data = server;

        VanityUrl = server.GetVanityUrlOrId();
    }

    public string Id { get; private set; }
    public ServerData Data { get; private set; }
    public string VanityUrl { get; set; }
    public bool IsVisible { get; set; } = true;
}
public class SelectedWebsiteData
{
    public SelectedWebsiteData(WebsiteData website)
    {
        Id = website.Id.ToString();
        Data = website;

        VanityUrl = website.GetVanityUrlOrId();
    }

    public string Id { get; private set; }
    public WebsiteData Data { get; private set; }
    public string VanityUrl { get; set; }
    public bool IsVisible { get; set; } = true;
}
public class SelectedProjectData
{
    public SelectedProjectData(ProjectData project)
    {
        Id = project.Id.ToString();
        Data = project;

        VanityUrl = project.GetVanityUrlOrId();
    }

    public string Id { get; private set; }
    public ProjectData Data { get; private set; }
    public string VanityUrl { get; set; }
    public bool IsVisible { get; set; } = true;
}