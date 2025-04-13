using Docker.DotNet.Models;

namespace DevSpaceWeb.API.Servers;

public class PluginJson
{
    public PluginJson(Plugin plugin)
    {
        id = plugin.ID;
        name = plugin.Name.Split(':').First();
        version = plugin.Name.Split(':').Last();
        is_enabled = plugin.Enabled;
        reference = plugin.PluginReference;
        if (plugin.PluginReference.StartsWith("docker.io"))
            url = $"https://hub.docker.com/r/{name}";
        else
            url = $"https://{plugin.PluginReference}";
        docker_version = plugin.Config.DockerVersion;
        description = plugin.Config.Description;
        documentation = plugin.Config.Documentation;
        work_directory = plugin.Config.WorkDir;
        if (plugin.Config.Entrypoint.Any())
            entrypoint = string.Join(" ", plugin.Config.Entrypoint);
        capabilities = plugin.Config.Linux.Capabilities;
    }

    public string id { get; set; }
    public string name { get; set; }
    public string version { get; set; }
    public bool is_enabled { get; set; }
    public string reference { get; set; }
    public string docker_version { get; set; }
    public string description { get; set; }
    public string url { get; set; }
    public string? documentation { get; set; }
    public string work_directory { get; set; }
    public string? entrypoint { get; set; }
    public IList<string> capabilities { get; set; } = new List<string>();
}
