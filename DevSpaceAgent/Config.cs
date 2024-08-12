using Newtonsoft.Json;

namespace DevSpaceAgent;

public class Config
{
    public string CertKey;
    public string AgentKey;

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
