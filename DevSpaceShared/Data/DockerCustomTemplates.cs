namespace DevSpaceShared.Data
{
    public class DockerCustomTemplatesList
    {
        public bool PortainerTemplatesFound { get; set; }
        public List<DockerCustomTemplate> Templates { get; set; }
    }

    public class DockerCustomTemplateData
    {
        public DockerCustomTemplate Template { get; set; }
        public string Compose { get; set; }
    }

    public class DockerCustomTemplate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public DockerCustomTemplatePlatformType PlatformType { get; set; }
        public DockerCustomTemplateServiceType ServiceType { get; set; }
    }
    public enum DockerCustomTemplatePlatformType
    {
        Linux, Windows
    }
    public enum DockerCustomTemplateServiceType
    {
        Standalone, Swarm
    }
}
