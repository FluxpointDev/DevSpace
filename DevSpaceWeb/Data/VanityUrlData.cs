﻿using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace DevSpaceWeb.Data;

public class VanityUrlData
{
    [BsonId]
    public ObjectId Id { get; set; }

    public ConcurrentDictionary<string, ObjectId> ServerVanityUrls = new ConcurrentDictionary<string, ObjectId>();

    public ConcurrentDictionary<string, ObjectId> ProjectVanityUrls = new ConcurrentDictionary<string, ObjectId>();

    public ConcurrentDictionary<string, ObjectId> WebsiteVanityUrls = new ConcurrentDictionary<string, ObjectId>();

    public ConcurrentDictionary<string, ObjectId> LogVanityUrls = new ConcurrentDictionary<string, ObjectId>();

    public ConcurrentDictionary<string, ObjectId> ConsoleVanityUrls = new ConcurrentDictionary<string, ObjectId>();

    public bool CheckAlreadyExists(ITeamResource? current, string? vanityUrl)
    {
        if (string.IsNullOrEmpty(vanityUrl))
            return false;

        if (ConsoleVanityUrls.TryGetValue(Utils.FormatVanityUrl(vanityUrl)!, out ObjectId consoleId) && (current == null || consoleId != current.Id))
            return true;

        return false;
    }

    public async Task UpdateAsync(ServerData server, string vanityUrl)
    {
        if (string.IsNullOrEmpty(vanityUrl))
        {
            // Update if vanity url is null and current is not null
            if (!string.IsNullOrEmpty(server.VanityUrl))
            {
                ServerVanityUrls.TryRemove(server.VanityUrl, out _);
                server.VanityUrl = null;
                await UpdateBaseAsync(new UpdateDefinitionBuilder<VanityUrlData>().Set(x => x.ServerVanityUrls, ServerVanityUrls), null!);
            }
        }
        else
        {
            // Update if console has no vanity url
            if (string.IsNullOrEmpty(server.VanityUrl))
            {
                ServerVanityUrls.TryAdd(vanityUrl, server.Id);
                server.VanityUrl = vanityUrl;
                await UpdateBaseAsync(new UpdateDefinitionBuilder<VanityUrlData>().Set(x => x.ServerVanityUrls, ServerVanityUrls), null!);
            }
            // Update if vanity url not match
            else if (Utils.FormatVanityUrl(vanityUrl) != server.VanityUrl)
            {
                ServerVanityUrls.TryRemove(server.VanityUrl!, out _);
                ServerVanityUrls.TryAdd(vanityUrl, server.Id);
                server.VanityUrl = vanityUrl;
                await UpdateBaseAsync(new UpdateDefinitionBuilder<VanityUrlData>().Set(x => x.ServerVanityUrls, ServerVanityUrls), null!);
            }
        }
    }

    public async Task UpdateAsync(ConsoleData console, string? vanityUrl)
    {
        if (string.IsNullOrEmpty(vanityUrl))
        {
            // Update if vanity url is null and current is not null
            if (!string.IsNullOrEmpty(console.VanityUrl))
            {
                ConsoleVanityUrls.TryRemove(console.VanityUrl, out _);
                console.VanityUrl = null;
                await UpdateBaseAsync(new UpdateDefinitionBuilder<VanityUrlData>().Set(x => x.ConsoleVanityUrls, ConsoleVanityUrls), null!);
            }
        }
        else
        {
            // Update if console has no vanity url
            if (string.IsNullOrEmpty(console.VanityUrl))
            {
                ConsoleVanityUrls.TryAdd(vanityUrl, console.Id);
                console.VanityUrl = vanityUrl;
                await UpdateBaseAsync(new UpdateDefinitionBuilder<VanityUrlData>().Set(x => x.ConsoleVanityUrls, ConsoleVanityUrls), null!);
            }
            // Update if vanity url not match
            else if (Utils.FormatVanityUrl(vanityUrl) != console.VanityUrl)
            {
                ConsoleVanityUrls.TryRemove(console.VanityUrl!, out _);
                ConsoleVanityUrls.TryAdd(vanityUrl, console.Id);
                console.VanityUrl = vanityUrl;
                await UpdateBaseAsync(new UpdateDefinitionBuilder<VanityUrlData>().Set(x => x.ConsoleVanityUrls, ConsoleVanityUrls), null!);
            }
        }
    }

    private async Task UpdateBaseAsync(UpdateDefinition<VanityUrlData> update, Action action)
    {
        FilterDefinition<VanityUrlData> filter = Builders<VanityUrlData>.Filter.Eq(r => r.Id, Id);
        UpdateResult Result = await _DB.TeamVanityUrls.Collection.UpdateOneAsync(filter, update);
        if (Result.IsAcknowledged)
            action?.Invoke();
    }
}
