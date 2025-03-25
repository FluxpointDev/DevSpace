using DevSpaceShared.Data;

namespace DevSpaceWeb.API.Servers;

public class NetworkJson
{
    public NetworkJson(DockerNetworkInfo network)
    {

    }

    public string id { get; set; }
    public string name { get; set; }
    public string driver { get; set; }
    public bool is_internal { get; set; }
    public bool is_attachable { get; set; }
    public string scope { get; set; }
    public bool ingress { get; set; }
    public bool config_only { get; set; }
    public DateTime created { get; set; }
    public IDictionary<string, string> labels { get; set; }
    public IDictionary<string, string> options { get; set; }
}
