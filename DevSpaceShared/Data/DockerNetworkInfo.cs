using Docker.DotNet.Models;

namespace DevSpaceShared.Data
{
    public class DockerNetworkInfo
    {
        public static DockerNetworkInfo Create(NetworkResponse network, bool inspect, NetworkResponse? networkFrom = null)
        {
            DockerNetworkInfo Info = new DockerNetworkInfo
            {
                Id = network.ID,
                Name = network.Name,
                Driver = network.Driver,
                IsInternal = network.Internal,
                Created = network.Created,
                IPAM = DockerNetworkIPAM.Create(network.IPAM),
                IsAttachable = network.Attachable,
                Scope = network.Scope,
                Ingress = network.Ingress,
                ConfigOnly = network.ConfigOnly,
            };
            if (network.Labels != null && network.Labels.TryGetValue("com.docker.compose.project", out string? project))
                Info.Stack = project;

            if (network.Containers != null)
            {
                Info.ContainersCount = network.Containers.Keys.Count();
                Info.Containers = network.Containers.ToDictionary(x => x.Key, x => x.Value.Name);
            }

            if (inspect)
            {
                Info.Labels = network.Labels;
                Info.Options = network.Options;
                Info.ContainersList = network.Containers;
                if (networkFrom != null)
                {
                    Info.ConfigFrom = new DockerNetworkInfo
                    {
                        Id = networkFrom.ID,
                        Name = networkFrom.Name
                    };
                }
            }

            return Info;
        }

        public required string Id { get; set; }
        public required string Name { get; set; }
        public string? Stack { get; set; }
        public string? Driver { get; set; }
        public bool IsInternal { get; set; }
        public bool IsAttachable { get; set; }
        public string? Scope { get; set; }
        public bool Ingress { get; set; }
        public bool ConfigOnly { get; set; }
        public DockerNetworkInfo? ConfigFrom { get; set; }
        public DateTime Created { get; set; }
        public IDictionary<string, string>? Labels { get; set; }
        public IDictionary<string, string>? Options { get; set; }
        public DockerNetworkIPAM? IPAM { get; set; }
        public Dictionary<string, string> Containers { get; set; } = [];

        public IDictionary<string, EndpointResource> ContainersList = new Dictionary<string, EndpointResource>();
        public int ContainersCount { get; set; }
    }
}
public class DockerNetworkIPAM
{
    public static DockerNetworkIPAM? Create(IPAM? ipam)
    {
        if (ipam == null)
            return null;

        DockerNetworkIPAM Info = new DockerNetworkIPAM
        {
            Driver = ipam.Driver,
            Options = ipam.Options
        };

        if (ipam.Config != null)
        {
            foreach (IPAMConfig? i in ipam.Config)
            {
                if (!string.IsNullOrWhiteSpace(i.Gateway) && i.Gateway.Contains(':'))
                {
                    Info.IPv6Subnet = i.Subnet;
                    Info.IPv6Gateway = i.Gateway;
                    Info.IPv6Range = i.IPRange;
                }
                else
                {
                    Info.IPv4Subnet = i.Subnet;
                    Info.IPv4Gateway = i.Gateway;
                    Info.IPv4Range = i.IPRange;
                }
            }
        }
        return Info;
    }

    public string? Driver { get; set; }

    public IDictionary<string, string>? Options { get; set; }

    public string? IPv4Subnet { get; set; }

    public string? IPv4Gateway { get; set; }

    public string? IPv4Range { get; set; }

    public string? IPv6Subnet { get; set; }

    public string? IPv6Gateway { get; set; }

    public string? IPv6Range { get; set; }
}
