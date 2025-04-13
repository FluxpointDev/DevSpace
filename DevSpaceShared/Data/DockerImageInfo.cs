using Docker.DotNet.Models;

namespace DevSpaceShared.Data
{
    public class DockerImageInfo
    {
        public static DockerImageInfo Create(ImageInspectResponse image)
        {
            string Name = "Unknown";
            if (image.RepoTags != null && image.RepoTags.Any())
                Name = image.RepoTags.First().Split(':').First();
            else if (image.RepoDigests != null && image.RepoDigests.Any())
                Name = image.RepoDigests.First().Split('@').First();

            DockerImageInfo Info = new DockerImageInfo
            {
                ID = image.ID,
                Name = Name,
                Created = image.Created,
                IsLocal = !string.IsNullOrEmpty(image.Parent),
                Size = image.Size,
                RepoTags = image.RepoTags,
                RepoDigests = image.RepoDigests,
                Parent = image.Parent,
                DockerVersion = image.DockerVersion,
                Os = image.Os,
                OsVersion = image.OsVersion,
                Architecture = image.Architecture,
                Comment = image.Comment,
                Driver = image.GraphDriver.Name,
                Config = DockerImageConfig.Create(image.Config)

            };

            string? AltVersion = null;
            string? Website = null;
            string? Docs = null;
            string? Source = null;
            if (image.Config.Labels != null)
            {
                image.Config.Labels.TryGetValue("org.opencontainers.image.url", out Website);
                image.Config.Labels.TryGetValue("org.opencontainers.image.url", out Docs);
                image.Config.Labels.TryGetValue("org.opencontainers.image.source", out Source);
                image.Config.Labels.TryGetValue("org.opencontainers.image.version", out AltVersion);
                if (!string.IsNullOrEmpty(AltVersion))
                    image.Config.Labels.TryGetValue("com.docker.compose.version", out AltVersion);
            }
            if (image.RepoTags != null && image.RepoTags.Any())
                Info.Version = image.RepoTags.First().Split(':').Last();

            Info.Website = Website;
            Info.Docs = Docs;
            Info.Source = Source;
            Info.AltVersion = AltVersion;

            return Info;
        }

        public static DockerImageInfo Create(ImagesListResponse image)
        {
            string Name = "Unknown";
            if (image.RepoTags != null && image.RepoTags.Any())
                Name = image.RepoTags.First().Split(':').First();
            else if (image.RepoDigests != null && image.RepoDigests.Any())
                Name = image.RepoDigests.First().Split('@').First();

            DockerImageInfo Info = new DockerImageInfo
            {
                ID = image.ID,
                Name = Name,
                Created = image.Created,
                IsLocal = !string.IsNullOrEmpty(image.ParentID),
                Size = image.Size
            };

            string? AltVersion = null;
            string? Website = null;
            string? Docs = null;
            string? Source = null;
            if (image.Labels != null)
            {
                image.Labels.TryGetValue("org.opencontainers.image.url", out Website);
                image.Labels.TryGetValue("org.opencontainers.image.url", out Docs);
                image.Labels.TryGetValue("org.opencontainers.image.source", out Source);
                image.Labels.TryGetValue("org.opencontainers.image.version", out AltVersion);
                if (!string.IsNullOrEmpty(AltVersion))
                    image.Labels.TryGetValue("com.docker.compose.version", out AltVersion);
            }
            if (image.RepoTags != null && image.RepoTags.Any())
                Info.Version = image.RepoTags.First().Split(':').Last();

            Info.Website = Website;
            Info.Docs = Docs;
            Info.Source = Source;
            Info.AltVersion = AltVersion;

            return Info;
        }

        public required string ID { get; set; }

        public required string Name { get; set; }

        public string? Version { get; set; }

        public string? AltVersion { get; set; }

        public DateTime Created { get; set; }

        public bool IsLocal { get; set; }

        public bool IsParent { get; set; }

        public string? Parent { get; set; }

        public long Size { get; set; }

        public Dictionary<string, string> Containers { get; set; } = new Dictionary<string, string>();
        public int ContainersCount { get; set; }

        public string? Website { get; set; }

        public string? Docs { get; set; }

        public string? Source { get; set; }

        public IList<string>? RepoTags { get; set; }

        public IList<string>? RepoDigests { get; set; }

        public string? DockerVersion { get; set; }

        public string? Os { get; set; }

        public string? Architecture { get; set; }

        public string? OsVersion { get; set; }

        public string? Comment { get; set; }

        public string? Driver { get; set; }

        public DockerImageConfig? Config { get; set; }
    }
}

public class DockerImageConfig
{
    public static DockerImageConfig Create(Config config)
    {
        DockerImageConfig Info = new DockerImageConfig
        {
            Environment = config.Env,
            Labels = config.Labels,
            WorkingDirectory = config.WorkingDir
        };

        if (config.Cmd != null)
            Info.Command = string.Join(" ", config.Cmd);

        if (config.Entrypoint != null)
            Info.Entrypoint = string.Join(" ", config.Entrypoint);

        if (config.ExposedPorts != null)
            Info.ExposedPorts = config.ExposedPorts.Keys.ToList();

        return Info;
    }
    public IList<string>? Environment { get; set; }

    public IDictionary<string, string>? Labels { get; set; }

    public string? Command { get; set; }

    public string? Entrypoint { get; set; }

    public IList<string>? ExposedPorts { get; set; }

    public string? WorkingDirectory { get; set; }
}
