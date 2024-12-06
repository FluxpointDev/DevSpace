using DevSpaceWeb.Data.Permissions;

namespace DevSpaceWeb;

public static class PermissionExtensions
{
    public static string GetName(this TeamPermission permission)
    {
        switch (permission)
        {
            case TeamPermission.GlobalAdministrator:
                return "Global Administrator";
            case TeamPermission.ManageMembers:
                return "Manage Members";
            case TeamPermission.AssignRoles:
                return "Assign Roles";
            case TeamPermission.ManagePermissions:
                return "Manage Permissions";
            case TeamPermission.ManageResources:
                return "Manage Resources";
            case TeamPermission.ManageRoles:
                return "Manage Roles";
            case TeamPermission.ManageTeam:
                return "Manage Team";
            case TeamPermission.TeamAdministrator:
                return "Team Administrator";
            case TeamPermission.ViewMembers:
                return "View Members";
            case TeamPermission.ViewPermissions:
                return "View Permissions";
            case TeamPermission.ViewAuditLogs:
                return "View Audit Logs";
        }
        return "Error!!";

    }
}
