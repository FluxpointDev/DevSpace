﻿using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Consoles;

public class ConsoleData : ITeamResource
{
    public string Ip { get; set; }
    public int Port { get; set; }
    public string EncryptedPassword { get; set; }
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

        return DecryptedPassword;
    }

    public List<DaRT.Player> RconPlayers()
    {
        if (_Data.BattleyeRcons.TryGetValue(Id, out var rcon))
        {
            return rcon.CachedPlayers;
        }

        return new List<DaRT.Player>();
    }

    public RconStatusType RconStatus()
    {
        if (_Data.BattleyeRcons.TryGetValue(Id, out var rcon))
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

    public async Task UpdateAsync(UpdateDefinition<ConsoleData> update, Action action)
    {
        FilterDefinition<ConsoleData> filter = Builders<ConsoleData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.Consoles.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }

    public async Task DeleteAsync(TeamMemberData member, Action action)
    {
        FilterDefinition<ConsoleData> filter = Builders<ConsoleData>.Filter.Eq(r => r.Id, Id);
        DeleteResult Result = await _DB.Consoles.Collection.DeleteOneAsync(filter);
        if (Result.IsAcknowledged)
        {
            _ = _DB.AuditLogs.CreateAsync(new AuditLog(member, AuditLogCategoryType.Resource, AuditLogEventType.ConsoleDeleted)
                .SetTarget(this));

            _DB.Consoles.Cache.TryRemove(Id, out _);
            switch (Type)
            {
                case ConsoleType.Battleye:
                    {
                        if (_Data.BattleyeRcons.TryGetValue(Id, out var rcon))
                        {
                            _Data.BattleyeRcons.Remove(Id);
                            rcon.Disconnect();
                        }
                    }
                    break;
                case ConsoleType.Minecraft:
                    {
                        if (_Data.MinecraftRcons.TryGetValue(Id, out var rcon))
                        {
                            _Data.MinecraftRcons.Remove(Id);
                            rcon.StopComms();
                        }
                    }
                    break;
            }

            action?.Invoke();
        }
    }
}

public enum ConsoleType
{
    Battleye, Minecraft
}

public enum RconStatusType
{
    Unknown, Online, Offline, Reconnecting, Error
}