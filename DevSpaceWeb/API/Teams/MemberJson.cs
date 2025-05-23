﻿using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.API.Teams;

public class MemberJson
{
    public MemberJson(TeamMemberData data, bool viewPerms)
    {
        id = data.Id.ToString();
        user_id = data.UserId.ToString();
        joined_at = data.JoinedAt;
        is_disabled = data.Disabled != null;
        color = data.GetCurrentColor();
        roles = data.GetCachedRoles().Select(x => new RoleJson(x, viewPerms)).ToArray();
        nickname = data.NickName;
        if (viewPerms)
            permissions = data.CalculatePermissions(null);
    }

    public string id;
    public string user_id;
    public bool is_disabled;
    public RoleJson[] roles;
    public string color;
    public string? nickname;
    public PermissionsSet? permissions;
    public DateTime joined_at;
}
