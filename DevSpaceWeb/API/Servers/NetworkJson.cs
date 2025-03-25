using DevSpaceShared.Data;

namespace DevSpaceWeb.API.Servers;

public class NetworkJson
{
    public NetworkJson(DockerNetworkInfo network)
    {
        id = network.Id;
        name = network.Name;
        driver = network.Driver;
        is_internal = network.IsInternal;
        is_attachable = network.IsAttachable;
        scope = network.Scope;
        ingress = network.Ingress;
        config_only = network.ConfigOnly;
        created_at = network.Created;
        labels = network.Labels;
        options = network.Options;
    }

    public string id { get; set; }
    public string name { get; set; }
    public string driver { get; set; }
    public bool is_internal { get; set; }
    public bool is_attachable { get; set; }
    public string scope { get; set; }
    public bool ingress { get; set; }
    public bool config_only { get; set; }
    public DateTime created_at { get; set; }
    public IDictionary<string, string> labels { get; set; }
    public IDictionary<string, string> options { get; set; }
}
