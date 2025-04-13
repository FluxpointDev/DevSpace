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

        if (network.ConfigFrom != null)
            config_from = new NetworkFromJson
            {
                id = network.ConfigFrom.Id,
                name = network.ConfigFrom.Name,
            };

        created_at = network.Created;

        if (network.Labels != null)
            labels = network.Labels;
        else
            labels = new Dictionary<string, string>();

        if (network.Options != null)
            options = network.Options;
        else
            options = new Dictionary<string, string>();

        containers_list = network.Containers;
        containers_count = containers_list.Count;
    }

    public string id { get; set; }
    public string name { get; set; }
    public string? driver { get; set; }
    public bool is_internal { get; set; }
    public bool is_attachable { get; set; }
    public string? scope { get; set; }
    public bool ingress { get; set; }
    public bool config_only { get; set; }
    public NetworkFromJson? config_from { get; set; }
    public DateTime created_at { get; set; }
    public IDictionary<string, string> labels { get; set; }
    public IDictionary<string, string> options { get; set; }
    public Dictionary<string, string> containers_list { get; set; }
    public int containers_count { get; set; }
}
public class NetworkFromJson
{
    public string id { get; set; }
    public string name { get; set; }
}