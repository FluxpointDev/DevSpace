using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DevSpaceWeb.Data;

public class PartialUserData
{
    public PartialUserData(AuthUser user)
    {
        Update(user);
    }

    public ObjectId Id { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }

    public string? GetCurrentName()
    {
        if (!string.IsNullOrEmpty(DisplayName))
            return DisplayName;

        return UserName;
    }
    public string? Email { get; set; }

    public Guid? AvatarId { get; set; }

    public string GetAvatarOrDefault(bool usePng = false)
    {
        if (!AvatarId.HasValue)
            return "https://cdn.fluxpoint.dev/devspace/user_avatar." + (usePng ? "png" : "webp");

        return _Data.Config.Instance.GetPublicUrl() + "/public/files/" + ResourceId.ToString() + "/Avatar_" + AvatarId.ToString() + ".webp";
    }

    public Guid? ResourceId { get; set; }
    public ObjectId? ManagedAccountTeamId { get; set; }

    public bool HasNotifications { get; set; }
    public List<SessionTask> Tasks { get; set; } = new List<SessionTask>();

    public void Update(AuthUser user)
    {
        Id = user.Id;
        UserName = user.UserName;
        DisplayName = user.DisplayName;
        Email = user.Email;
        AvatarId = user.AvatarId;
        ResourceId = user.ResourceId;
        ManagedAccountTeamId = user.Account.ManagedAccountTeamId;
        HasNotifications = user.Account.HasNotifications;
    }

    public event NotificationEventHandler NotificationTriggered;

    public void TriggerNotificationEvent(Notification notification)
    {
        HasNotifications = true;
        NotificationTriggered?.Invoke(notification);
    }

    public async Task AddNotification(NotificationType type, TeamData? team)
    {
        Notification notification = new Notification { Type = type, TeamId = team?.Id, UserId = Id };
        await _DB.Notifications.CreateAsync(notification);
        HasNotifications = true;
        FilterDefinition<AuthUser> filter = new FilterDefinitionBuilder<AuthUser>().Eq(x => x.Id, Id);
        UpdateDefinition<AuthUser> update = new UpdateDefinitionBuilder<AuthUser>().Set(x => x.Account.HasNotifications, true);
        _ = _DB.Run.GetCollection<AuthUser>("users").UpdateOneAsync(filter, update);

        NotificationTriggered?.Invoke(notification);
    }

    public async Task ClearNotifications()
    {
        HasNotifications = false;
        FilterDefinition<Notification> filter = Builders<Notification>.Filter.Eq(r => r.UserId, Id);
        await _DB.Notifications.Collection.DeleteManyAsync(filter);
        FilterDefinition<AuthUser> filterUser = new FilterDefinitionBuilder<AuthUser>().Eq(x => x.Id, Id);
        UpdateDefinition<AuthUser> update = new UpdateDefinitionBuilder<AuthUser>().Set(x => x.Account.HasNotifications, false);
        _ = _DB.Run.GetCollection<AuthUser>("users").UpdateOneAsync(filterUser, update);
    }
}
