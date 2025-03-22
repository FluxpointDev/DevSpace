using DevSpaceShared.Data;
using DevSpaceShared.Events.Docker;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

namespace DevSpaceAgent.Docker;

public static class DockerCustomTemplates
{
    public static async Task<DockerCustomTemplatesList> ListTemplatesAsync()
    {
        bool HasPortainerTemplates = false;
        try
        {
            if (Directory.GetDirectories("/var/lib/docker/volumes/portainer_data/_data/custom_templates").Any())
                HasPortainerTemplates = true;
        }
        catch { }

        return new DockerCustomTemplatesList
        {
            PortainerTemplatesFound = HasPortainerTemplates,
            Templates = Program.CustomTemplates.Values.ToList()
        };
    }

    public static async Task CreateTemplateAsync(CreateCustomTemplateEvent data)
    {
        string Id = Guid.NewGuid().ToString().Replace("-", "");
        Program.CustomTemplates.Add(Id, new DockerCustomTemplate
        {
            Id = Id
        });
        Program.SaveTemplates();
        Directory.CreateDirectory(Program.CurrentDirectory + "Data/Templates/" + Id);
    }

    public static async Task<DockerCustomTemplate?> GetTemplateInfoAsync(string id)
    {
        if (Program.CustomTemplates.TryGetValue(id, out DockerCustomTemplate template))
            return template;
        return null;
    }

    public static async Task<DockerCustomTemplateData?> GetTemplateAsync(string id)
    {
        if (Program.CustomTemplates.TryGetValue(id, out DockerCustomTemplate template))
        {
            string Compose = File.ReadAllText(Program.CurrentDirectory + "Data/Templates/" + id + "/docker-compose.yml");
            return new DockerCustomTemplateData
            {
                Template = template,
                Compose = Compose
            };
        }
        return null;
    }

    public static async Task<string> GetTemplateDataAsync(string id)
    {
        return File.ReadAllText(Program.CurrentDirectory + "Data/Templates/" + id + "/docker-compose.yml");
    }

    public static async Task ImportPortainerTemplatesAsync()
    {
        foreach (var i in Directory.GetDirectories("/var/lib/docker/volumes/portainer_data/_data/custom_templates"))
        {
            if (File.Exists(i + "/docker-compose.yml"))
            {
                string Data = File.ReadAllText(i + "/docker-compose.yml");
                string Id = Guid.NewGuid().ToString().Replace("-", "");

                var deserializer = new DeserializerBuilder().Build();
                var yamlObject = deserializer.Deserialize(Data);

                var serializer = new SerializerBuilder()
                    .JsonCompatible()
                    .Build();

                var json = serializer.Serialize(yamlObject);
                var JO = JObject.Parse(json);
                string? Name = null;
                try
                {
                    Name = JO["services"].First().Path.Replace("services.", "");
                }
                catch { }

                Program.CustomTemplates.Add(Id, new DockerCustomTemplate
                {
                    Id = Id,
                    Name = Name
                });
                Program.SaveTemplates();
                Directory.CreateDirectory(Program.CurrentDirectory + "Data/Templates/" + Id);
                File.WriteAllText(Program.CurrentDirectory + "Data/Templates/" + Id + "/docker-compose.yml", Data);
            }
        }

        Console.WriteLine("Template: " + Program.CustomTemplates.Values.Count());
    }

    public static async Task EditTemplateAsync(string id, EditCustomTemplateInfoEvent data)
    {
        if (Program.CustomTemplates.TryGetValue(id, out var template))
        {
            Program.SaveTemplates();
        }
        else
        {
            Program.CustomTemplates.Add(id, new DockerCustomTemplate
            {
                Id = id
            });
            Program.SaveTemplates();
        }
    }

    public static async Task EditTemplateData(string id, EditCustomTemplateComposeEvent data)
    {
        File.WriteAllText(Program.CurrentDirectory + "Data/Templates/" + id + "/docker-compose.yml", data.Data);
    }

    public static async Task DeleteTemplateAsync(string id)
    {
        Program.CustomTemplates.Remove(id);
        Program.SaveTemplates();
        Directory.Delete(Program.CurrentDirectory + "Data/Templates/" + id);
    }

    public static async Task<object?> ControlTemplateAsync(DockerEvent @event)
    {
        switch (@event.CustomTemplateType)
        {
            case ControlCustomTemplateType.ViewInfo:
                return await GetTemplateAsync(@event.ResourceId);
            case ControlCustomTemplateType.ComposeInfo:
                return await GetTemplateDataAsync(@event.ResourceId);
            case ControlCustomTemplateType.ViewFull:
                return await GetTemplateAsync(@event.ResourceId);
            case ControlCustomTemplateType.EditInfo:
                await EditTemplateAsync(@event.ResourceId, @event.Data.ToObject<EditCustomTemplateInfoEvent>());
                break;
            case ControlCustomTemplateType.EditCompose:
                await EditTemplateData(@event.ResourceId, @event.Data.ToObject<EditCustomTemplateComposeEvent>());
                break;
            case ControlCustomTemplateType.Delete:
                await DeleteTemplateAsync(@event.ResourceId);
                break;
        }

        return null;
    }
}
