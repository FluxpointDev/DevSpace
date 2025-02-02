using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.API.Teams;

public class MemberJson
{
    public MemberJson(TeamMemberData data, bool viewPerms)
    {
        id = data.Id.ToString();
        user_id = data.UserId.ToString();
        team_id = data.TeamId.ToString();
        joined_at = data.JoinedAt;
        is_disabled = data.Disabled != null;
        roles = data.Roles.Select(x => x.ToString()).ToArray();
        nickname = data.NickName;
        if (viewPerms)
            permissions = data.CalculatePermissions(null);
    }

    public string id { get; set; }
    public string user_id { get; set; }
    public string team_id { get; set; }
    public bool is_disabled { get; set; }
    public string[] roles { get; set; }
    public string? nickname { get; set; }
    public PermissionsSet? permissions { get; set; }
    public DateTime joined_at { get; set; }
}
