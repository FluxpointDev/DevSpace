using System.Text.Json.Serialization;

namespace Docker.DotNet.Models;

public class VolumeAccessMode // (volume.AccessMode)
{
    [JsonPropertyName("Scope")]
    public string Scope { get; set; }

    [JsonPropertyName("Sharing")]
    public string Sharing { get; set; }

    [JsonPropertyName("MountVolume")]
    public TypeMount MountVolume { get; set; }

    [JsonPropertyName("BlockVolume")]
    public TypeBlock BlockVolume { get; set; }
}

