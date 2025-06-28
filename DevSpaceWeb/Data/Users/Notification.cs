using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson;

namespace DevSpaceWeb.Data.Users;

public class Notification : IObject
{
    public ObjectId UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ObjectId? TeamId { get; set; }
    public NotificationType Type;

    public string? GetIcon()
    {
        if (TeamId.HasValue)
        {
            if (_DB.Teams.Cache.TryGetValue(TeamId.Value, out Teams.TeamData? team))
                return team.GetIconOrDefault();
        }
        return null;
    }

    public string? GetText()
    {
        switch (Type)
        {
            case NotificationType.InvitedToTeam:
                if (TeamId.HasValue && _DB.Teams.Cache.TryGetValue(TeamId.Value, out Teams.TeamData? team))
                    return $"You have been invited to join the {team.Name} team.";
                return "Invalid invite.";
        }
        return null;
    }
}
public enum NotificationType
{
    InvitedToTeam
}
