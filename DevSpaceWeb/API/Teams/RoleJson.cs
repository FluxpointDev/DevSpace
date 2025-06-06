﻿using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.API.Teams;

public class RoleJson
{
    public RoleJson(TeamRoleData data, bool viewPermissions)
    {
        id = data.Id.ToString();
        created_at = data.CreatedAt;
        name = data.Name;
        description = data.Description;
        position = data.GetPosition();
        if (viewPermissions)
            permissions = data.Permissions;
        color = data.Color;
    }

    public string id;
    public string? name;
    public string? description;
    public int position;
    public PermissionsSet? permissions;
    public DateTime created_at;
    public string? color;
}
