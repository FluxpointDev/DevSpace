using Docker.DotNet.Models;

namespace DevSpaceShared.Data;
public class DockerContainerNetwork
{
    public static DockerContainerNetwork Create(string name, EndpointSettings network)
    {
        return new DockerContainerNetwork
        {
            Id = network.NetworkID,
            Name = name,
            Gateway = network.Gateway,
            IPAddress = network.IPAddress,
            MacAddress = network.MacAddress,
        };
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public string IPAddress { get; set; }
    public string Gateway { get; set; }
    public string MacAddress { get; set; }
}
