using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DevSpaceWeb.Apps.Data;

public class WorkspaceData : IObject
{
    public ObjectId AppId { get; set; }
    public ObjectId TeamId { get; set; }
    public string CommandFormat { get; set; }
    public string ServerId { get; set; }
    public string? JsonData { get; set; }
    public WorkspaceType Type { get; set; }
    public bool IsPublic { get; set; }

    public async Task<UpdateResult> UpdateAsync(UpdateDefinition<WorkspaceData> update, Action? action = null)
    {
        FilterDefinition<WorkspaceData> filter = Builders<WorkspaceData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Workspaces.Collection.UpdateOneAsync(filter, update);
        if (action != null && Result.IsAcknowledged)
            action?.Invoke();
        return Result;
    }
}
public enum WorkspaceType
{
    DiscordSlashCommand,
    DiscordUserCommand,
    DiscordMessageCommand,
    DiscordInteractionButton,
    DiscordInteractionModal,
    DiscordInteractionSelectString,
    DiscordInteractionSelectUser,
    DiscordInteractionSelectRole,
    DiscordInteractionSelectChannel,
    DiscordInteractionSelectMentionable
}