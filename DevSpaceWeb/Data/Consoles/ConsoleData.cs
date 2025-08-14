using DaRT;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using LibMCRcon;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Consoles;

public class ConsoleData : ITeamResource
{
    public ConsoleData() : base(ResourceType.Console) { }

    public required string Ip { get; set; }
    public required short Port { get; set; }
    public required string EncryptedPassword { get; set; }
    public ConsoleType Type { get; set; }
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

    [BsonIgnore]
    private string? DecryptedPassword;

    public void ResetDecryptedPassword()
    {
        DecryptedPassword = null;
    }
    public string GetDecryptedPassword()
    {
        //if (string.IsNullOrEmpty(DecryptedPassword))
        //    DecryptedPassword = Crypt.DecryptString(EncryptedPassword);

        return EncryptedPassword;
    }

    public bool IsOnline()
    {
        switch (Type)
        {
            case ConsoleType.Battleye:
                {
                    if (_Data.BattleyeRcons.TryGetValue(Id, out BattlEyeRcon? rcon) && rcon.BEResult == BattleNET.BattlEyeConnectionResult.Success)
                        return true;
                }
                break;
            case ConsoleType.Minecraft:
                {
                    if (_Data.MinecraftRcons.TryGetValue(Id, out MCRconAsync? rcon) && rcon.IsConnected)
                        return true;
                }
                break;
            case ConsoleType.Source:
                {
                    if (_Data.SourceRcons.TryGetValue(Id, out CoreRCON.RCON? rcon) && rcon.Connected)
                        return true;
                }
                break;
        }

        return false;
    }

    public List<DaRT.Player> RconPlayers()
    {
        if (_Data.BattleyeRcons.TryGetValue(Id, out BattlEyeRcon? rcon))
        {
            return rcon.CachedPlayers;
        }

        return [];
    }

    public RconStatusType RconStatus()
    {
        if (_Data.BattleyeRcons.TryGetValue(Id, out BattlEyeRcon? rcon))
        {
            if (rcon.IsError)
                return RconStatusType.Error;

            if (rcon.IsReconnecting)
                return RconStatusType.Reconnecting;

            if (rcon.IsConnected)
                return RconStatusType.Online;

            return RconStatusType.Offline;
        }

        return RconStatusType.Unknown;
    }



    public async Task<bool> UpdateAsync(UpdateDefinition<ConsoleData> update, Action action)
    {
        FilterDefinition<ConsoleData> filter = Builders<ConsoleData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Consoles.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();

        return Result.IsAcknowledged;
    }


    public override async Task<bool> DeleteAsync(TeamMemberData? member, Action? action = null)
    {
        FilterDefinition<ConsoleData> filter = Builders<ConsoleData>.Filter.Eq(r => r.Id, Id);
        DeleteResult Result = await _DB.Consoles.Collection.DeleteOneAsync(filter);
        if (Result.IsAcknowledged)
        {
            if (member != null)
                _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Resource, AuditLogEventType.ConsoleDeleted)
                .SetTarget(this));

            _DB.Consoles.Cache.TryRemove(Id, out _);
            switch (Type)
            {
                case ConsoleType.Battleye:
                    {
                        if (_Data.BattleyeRcons.TryGetValue(Id, out BattlEyeRcon? rcon))
                        {
                            _Data.BattleyeRcons.Remove(Id);
                            rcon.Disconnect();
                        }
                    }
                    break;
                case ConsoleType.Minecraft:
                    {
                        if (_Data.MinecraftRcons.TryGetValue(Id, out MCRconAsync? rcon))
                        {
                            _Data.MinecraftRcons.Remove(Id);
                            await rcon.StopComms();
                        }
                    }
                    break;
            }

            action?.Invoke();
        }

        return Result.IsAcknowledged;
    }
}

public enum ConsoleType
{
    Battleye, Minecraft, Source
}

public enum RconStatusType
{
    Unknown, Online, Offline, Reconnecting, Error
}
