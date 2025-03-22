namespace DevSpaceShared.Events.Docker
{
    public class CreateImageEvent
    {
        public Dictionary<string, string> Labels { get; set; }
        public string Content { get; set; }

        public CreateImageType Type { get; set; }

        public string RemoteUrl { get; set; }
        public string DockerfileName { get; set; }
    }
    public enum CreateImageType
    {
        Editor, Upload, Remote
    }
}
