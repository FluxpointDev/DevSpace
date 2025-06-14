using DevSpaceShared.Agent;
using Newtonsoft.Json;

namespace DevSpaceAgent;

public class Config
{
    public string? CertKey;
    public string? AgentId;
    public string? AgentKey;

    public string? EdgeIp;
    public short EdgePort = 443;
    public string EdgeId;
    public string EdgeKey;
    public HashSet<string> AllowedIPs = [];
    public AgentOptions Options = new AgentOptions();
    public void Save()
    {
        using (StreamWriter file = File.CreateText(Program.CurrentDirectory + $"Data/Config.json"))
        {
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            serializer.Serialize(file, this);
        }
    }
}
