using DevSpaceWeb.Apps.Data;
using DevSpaceWeb.Data.API;
using DevSpaceWeb.Data.Consoles;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Data.Reports;
using DevSpaceWeb.Data.Servers;
using DevSpaceWeb.Data.Websites;
using DevSpaceWeb.Database;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Teams;

public static class TeamFunctions
{
    public static async Task<bool> DeleteTeamAsync(TeamData team)
    {
        // Delete roles
        FilterDefinition<TeamRoleData> RolesFilter = Builders<TeamRoleData>.Filter.Eq(r => r.TeamId, team.Id);
        DeleteResult RolesResult = await _DB.Roles.Collection.DeleteManyAsync(RolesFilter);

        if (RolesResult.IsAcknowledged)
        {
            team.CachedRoles.Clear();
            foreach (TeamRoleData? i in _DB.Roles.Cache.Values.Where(x => x.TeamId == team.Id))
            {
                _DB.Roles.Cache.TryRemove(i.Id, out _);
            }
        }
        else
        {
            return false;
        }

        // Delete apis
        FilterDefinition<APIClient> APIFilter = Builders<APIClient>.Filter.Eq(r => r.TeamId, team.Id);
        await _DB.API.Collection.DeleteManyAsync(APIFilter);

        // Delete servers
        FilterDefinition<ServerData> ServerFilter = Builders<ServerData>.Filter.Eq(r => r.TeamId, team.Id);
        await _DB.Servers.Collection.DeleteManyAsync(ServerFilter);

        foreach (ServerData? i in _DB.Servers.Cache.Values.Where(x => x.TeamId == team.Id))
        {
            _DB.Servers.Cache.TryRemove(i.Id, out _);
            try
            {
                i.RemoveWebSocket();
            }
            catch { }
        }

        // Delete apps
        FilterDefinition<AppData> AppFilter = Builders<AppData>.Filter.Eq(r => r.TeamId, team.Id);
        await _DB.Apps.Collection.DeleteManyAsync(AppFilter);

        foreach (AppData? i in _DB.Apps.Cache.Values.Where(x => x.TeamId == team.Id))
        {
            _DB.Apps.Cache.TryRemove(i.Id, out _);
            if (_Data.DiscordClients.TryGetValue(i.Id, out Discord.Rest.DiscordRestClient? client))
            {
                try
                {
                    client.Dispose();
                }
                catch { }
                _Data.DiscordClients.Remove(i.Id);
            }
        }

        FilterDefinition<WorkspaceData> WorkspaceFilter = Builders<WorkspaceData>.Filter.Eq(r => r.TeamId, team.Id);
        await _DB.Workspaces.Collection.DeleteManyAsync(WorkspaceFilter);


        // Delete consoles
        FilterDefinition<ConsoleData> ConsoleFilter = Builders<ConsoleData>.Filter.Eq(r => r.TeamId, team.Id);
        await _DB.Consoles.Collection.DeleteManyAsync(ConsoleFilter);

        foreach (ConsoleData? i in _DB.Consoles.Cache.Values.Where(x => x.TeamId == team.Id))
        {
            _DB.Consoles.Cache.TryRemove(i.Id, out _);
        }

        // Delete projects
        FilterDefinition<ProjectData> ProjectFilter = Builders<ProjectData>.Filter.Eq(r => r.TeamId, team.Id);
        await _DB.Projects.Collection.DeleteManyAsync(ProjectFilter);

        foreach (ProjectData? i in _DB.Projects.Cache.Values.Where(x => x.TeamId == team.Id))
        {
            _DB.Projects.Cache.TryRemove(i.Id, out _);
        }

        // Delete websites
        FilterDefinition<WebsiteData> WebFilter = Builders<WebsiteData>.Filter.Eq(r => r.TeamId, team.Id);
        await _DB.Websites.Collection.DeleteManyAsync(WebFilter);

        foreach (WebsiteData? i in _DB.Websites.Cache.Values.Where(x => x.TeamId == team.Id))
        {
            _DB.Websites.Cache.TryRemove(i.Id, out _);
        }

        // Delete Vanity
        FilterDefinition<VanityUrlData> VanityFilter = Builders<VanityUrlData>.Filter.Eq(r => r.Id, team.Id);
        await _DB.TeamVanityUrls.Collection.DeleteOneAsync(VanityFilter);
        if (_DB.TeamVanityUrls.Cache.TryRemove(team.Id, out VanityUrlData? vanityData))
        {
            vanityData.ClearCache();
        }

        if (!string.IsNullOrEmpty(team.VanityUrl))
            _DB.TeamsVanityCache.TryRemove(team.VanityUrl, out _);

        // Delete members
        FilterDefinition<TeamMemberData> MembersFilter = Builders<TeamMemberData>.Filter.Eq(r => r.TeamId, team.Id);
        await _DB.Members.Collection.DeleteManyAsync(MembersFilter);

        foreach (TeamMemberData? i in _DB.Members.Cache.Values.Where(x => x.TeamId == team.Id))
        {
            _DB.Members.Cache.TryRemove(i.Id, out _);
        }

        // Delete Team
        FilterDefinition<TeamData> TeamFilter = Builders<TeamData>.Filter.Eq(r => r.Id, team.Id);
        await _DB.Teams.Collection.DeleteOneAsync(TeamFilter);

        _DB.Teams.Cache.TryRemove(team.Id, out _);

        // Background cleanups
        _ = Task.Run(async () =>
        {
            // Delete error logs
            FilterDefinition<LogData> LogFilter = Builders<LogData>.Filter.Eq(r => r.TeamId, team.Id);
            await _DB.Logs.Collection.DeleteManyAsync(LogFilter);

            // Delete audit logs
            FilterDefinition<AuditLog> AuditFilter = Builders<AuditLog>.Filter.Eq(r => r.TeamId, team.Id);
            await _DB.AuditLogs.Collection.DeleteManyAsync(AuditFilter);
        });

        return true;
    }
}
