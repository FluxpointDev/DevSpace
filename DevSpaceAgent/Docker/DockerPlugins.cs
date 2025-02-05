using DevSpaceShared.Events.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DevSpaceAgent.Docker
{
    public static class DockerPlugins
    {
        public static async Task<IList<Plugin>> ListPluginsAsync(DockerClient client)
        {
            return await client.Plugin.ListPluginsAsync(new PluginListParameters
            {

            });
        }

        public static async Task<object?> ControlPluginAsync(DockerClient client, DockerEvent @event)
        {
            switch (@event.PluginType)
            {
                case ControlPluginType.InstallCheck:
                    return await client.Plugin.GetPluginPrivilegesAsync(new PluginGetPrivilegeParameters
                    {
                        Remote = @event.ResourceId
                    });
                case ControlPluginType.InstallFull:
                    {
                        IList<PluginPrivilege> Privs = await client.Plugin.GetPluginPrivilegesAsync(new PluginGetPrivilegeParameters
                        {
                            Remote = @event.ResourceId
                        });
                        string ErrorMessage = "";
                        Progress<JSONMessage> progress = new Progress<JSONMessage>(msg =>
                        {
                            ErrorMessage = msg.ErrorMessage;
                        });
                        await client.Plugin.InstallPluginAsync(new PluginInstallParameters
                        {
                            Remote = @event.ResourceId,
                            Privileges = Privs
                        }, progress);

                        if (!string.IsNullOrEmpty(ErrorMessage))
                            throw new Exception(ErrorMessage);
                    }
                    break;
                case ControlPluginType.Enable:
                    {
                        await client.Plugin.EnablePluginAsync(@event.ResourceId, new PluginEnableParameters
                        {

                        });
                    }
                    break;
                case ControlPluginType.Disable:
                    {
                        await client.Plugin.DisablePluginAsync(@event.ResourceId, new PluginDisableParameters
                        {

                        });
                    }
                    break;
                case ControlPluginType.Remove:
                    {
                        await client.Plugin.RemovePluginAsync(@event.ResourceId, new PluginRemoveParameters
                        {

                        });
                    }
                    break;
                case ControlPluginType.Update:
                    {
                        await client.Plugin.UpgradePluginAsync(@event.ResourceId, new PluginUpgradeParameters
                        {
                            Remote = @event.ResourceId
                        });
                    }
                    break;
            }

            return null;
        }
    }
}
