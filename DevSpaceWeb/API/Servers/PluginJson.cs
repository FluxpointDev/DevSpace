using Docker.DotNet.Models;

namespace DevSpaceWeb.API.Servers;

public class PluginJson
{
    public PluginJson(Plugin plugin)
    {
        id = plugin.ID;
        name = plugin.Name;
        is_enabled = plugin.Enabled;
        reference = plugin.PluginReference;
        docker_version = plugin.Config.DockerVersion;
        description = plugin.Config.Description;
        documentation = plugin.Config.Documentation;
    }

    public string id;
    public string name;
    public bool is_enabled;
    public string reference;
    public string docker_version;
    public string description;
    public string? documentation;
}
