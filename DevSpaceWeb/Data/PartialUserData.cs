using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using Radzen;

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
        {
            if (UseGravatar)
                return GetGravatarOrDefault();

            return "https://cdn.fluxpoint.dev/devspace/default_avatar." + (usePng ? "png" : "webp");
        }

        return _Data.Config.Instance.GetPublicUrl() + "/public/files/" + ResourceId.ToString() + "/Avatar_" + AvatarId.ToString() + ".webp";
    }

    public string GetGravatarOrDefault()
    {
        string md5Email = MD5.Calculate(System.Text.Encoding.ASCII.GetBytes(Email != null ? Email : ""));

        return $"https://secure.gravatar.com/avatar/{md5Email}?d=mp&s=128";
    }

    public Guid? ResourceId { get; set; }
    public ObjectId? ManagedAccountTeamId { get; set; }
    public bool HasNotifications { get; set; }
    public bool Has2FA { get; set; }
    public bool UseGravatar { get; set; }

    public void Update(AuthUser user)
    {
        Id = user.Id;
        UserName = user.UserName;
        DisplayName = user.DisplayName;
        Email = user.Email;
        AvatarId = user.AvatarId;
        UseGravatar = user.UseGravatar;
        ResourceId = user.ResourceId;
        ManagedAccountTeamId = user.Account.ManagedAccountTeamId;
        HasNotifications = user.Account.HasNotifications;
        Has2FA = user.Mfa.HasAny2FA();
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
