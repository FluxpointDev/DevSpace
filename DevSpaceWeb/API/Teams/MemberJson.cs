using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.API.Teams;

public class MemberJson
{
    public MemberJson(TeamMemberData data)
    {
        id = data.Id.ToString();
        user_id = data.UserId.ToString();
        team_id = data.TeamId.ToString();
        is_disabled = data.Disabled != null;
        roles = data.Roles.Select(x => x.ToString()).ToArray();
        nickname = data.NickName;
    }

    public string id { get; set; }
    public string user_id { get; set; }
    public string team_id { get; set; }
    public bool is_disabled { get; set; }
    public string[] roles { get; set; }
    public string? nickname { get; set; }
}
