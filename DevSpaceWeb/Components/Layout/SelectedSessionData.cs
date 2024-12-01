using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Websites;

namespace DevSpaceWeb.Components.Layout;

public class SelectedSessionData
{
    public SelectedTeamData Team { get; set; }

    public TeamMemberData? Member { get { return Team != null && Team.Member != null ? Team.Member : null; } }

    public ServerData? Server { get { return Team != null && Team.Server != null ? Team.Server.Data : null; } }

    public WebsiteData? Website { get { return Team != null && Team.Website != null ? Team.Website.Data : null; } }

    public ProjectData? Project { get { return Team != null && Team.Project != null ? Team.Project.Data : null; } }

    public LogData? Log { get { return Team != null && Team.Log != null ? Team.Log.Data : null; } }
}
public class SelectedTeamData
{
    public SelectedTeamData(TeamData team)
    {
        Id = team.Id.ToString();
        Data = team;

        if (!string.IsNullOrEmpty(team.VanityUrl))
            VanityUrl = team.VanityUrl;
        else
            VanityUrl = Id;
    }

    public string Id { get; private set; }
    public TeamData Data { get; private set; }
    
    public string VanityUrl { get; set; }

    public TeamMemberData Member { get; set; }
    public SelectedServerData Server { get; set; }
    public SelectedWebsiteData Website { get; set; }
    public SelectedProjectData Project { get; set; }
    public SelectedLogData Log { get; set; }
}

public class SelectedServerData
{
    public SelectedServerData(SelectedTeamData selectedTeam, ServerData server)
    {
        Id = server.Id.ToString();
        Data = server;

        VanityUrl = server.GetVanityUrl();
    }

    public string Id { get; private set; }
    public ServerData Data { get; private set; }
    public string VanityUrl { get; set; }
}
public class SelectedWebsiteData
{
    public SelectedWebsiteData(SelectedTeamData selectedTeam, WebsiteData website)
    {
        Id = website.Id.ToString();
        Data = website;

        VanityUrl = website.GetVanityUrl();
    }

    public string Id { get; private set; }
    public WebsiteData Data { get; private set; }
    public string VanityUrl { get; set; }
}
public class SelectedProjectData
{
    public SelectedProjectData(SelectedTeamData selectedTeam, ProjectData project)
    {
        Id = project.Id.ToString();
        Data = project;

        VanityUrl = project.GetVanityUrl();
    }

    public string Id { get; private set; }
    public ProjectData Data { get; private set; }
    public string VanityUrl { get; set; }
}
public class SelectedLogData
{
    public SelectedLogData(SelectedTeamData selectedTeam, LogData log)
    {
        Id = log.Id.ToString();
        Data = log;

        VanityUrl = log.GetVanityUrl();
    }

    public string Id { get; private set; }
    public LogData Data { get; private set; }
    public string VanityUrl { get; set; }
}