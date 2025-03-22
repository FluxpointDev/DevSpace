using DevSpaceShared.Events.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker;

public static class DockerPlugins
{
    public static async Task<IList<Plugin>> ListPluginsAsync(DockerClient client)
    {
        return await client.Plugin.ListPluginsAsync(new PluginListParameters
        {

        });
    }

    public static async Task<object?> ControlPluginAsync(DockerClient client, DockerEvent @event, string id)
    {
        switch (@event.PluginType)
        {
            case ControlPluginType.Inspect:
                return await client.Plugin.InspectPluginAsync(id);
            case ControlPluginType.InstallCheck:
                return await client.Plugin.GetPluginPrivilegesAsync(new PluginGetPrivilegeParameters
                {
                    Remote = id
                });
            case ControlPluginType.InstallFull:
                {
                    IList<PluginPrivilege> Privs = await client.Plugin.GetPluginPrivilegesAsync(new PluginGetPrivilegeParameters
                    {
                        Remote = id
                    });
                    string ErrorMessage = "";
                    Progress<JSONMessage> progress = new Progress<JSONMessage>(msg =>
                    {
                        ErrorMessage = msg.ErrorMessage;
                    });
                    await client.Plugin.InstallPluginAsync(new PluginInstallParameters
                    {
                        Remote = id,
                        Privileges = Privs
                    }, progress);

                    if (!string.IsNullOrEmpty(ErrorMessage))
                        throw new Exception(ErrorMessage);
                }
                break;
            case ControlPluginType.Enable:
                {
                    await client.Plugin.EnablePluginAsync(id, new PluginEnableParameters
                    {

                    });
                }
                break;
            case ControlPluginType.Disable:
                {
                    await client.Plugin.DisablePluginAsync(id, new PluginDisableParameters
                    {

                    });
                }
                break;
            case ControlPluginType.ForceRemove:
            case ControlPluginType.Remove:
                {
                    await client.Plugin.RemovePluginAsync(id, new PluginRemoveParameters
                    {
                        Force = @event.PluginType == ControlPluginType.ForceRemove
                    });
                }
                break;
            case ControlPluginType.Update:
                {
                    IList<PluginPrivilege> Privs = await client.Plugin.GetPluginPrivilegesAsync(new PluginGetPrivilegeParameters
                    {
                        Remote = id
                    });
                    await client.Plugin.UpgradePluginAsync(@event.ResourceId, new PluginUpgradeParameters
                    {
                        Remote = id,
                        Privileges = Privs
                    });
                }
                break;
        }

        return null;
    }
}
