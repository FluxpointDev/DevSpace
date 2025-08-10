using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.Components.Layout;

public class SelectedSessionData
{
    public SelectedTeamData? Team { get; set; }

    public TeamMemberData? Member { get { return Team != null && Team.Member != null ? Team.Member : null; } }

    public ITeamResource? Resource { get { return Team != null && Team.Resource != null ? Team.Resource.Data : null; } }
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
    public SelectedResourceData? Resource { get; set; }
}

public class SelectedResourceData
{
    public SelectedResourceData(ITeamResource data)
    {
        Id = data.Id.ToString();
        Data = data;

        VanityUrl = data.GetVanityUrlOrId();
    }

    public string Id { get; private set; }
    public ITeamResource Data { get; private set; }
    public string VanityUrl { get; set; }
}