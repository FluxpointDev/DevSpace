using DevSpaceShared.Data;
using DevSpaceShared.Events.Docker;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

namespace DevSpaceAgent.Docker;

public static class DockerCustomTemplates
{
    public static DockerCustomTemplatesList ListTemplates()
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

    public static void CreateTemplate(CreateCustomTemplateEvent data)
    {
        string Id = Guid.NewGuid().ToString().Replace("-", "");
        Program.CustomTemplates.Add(Id, new DockerCustomTemplate
        {
            Id = Id
        });
        Program.SaveTemplates();
        Directory.CreateDirectory(Program.CurrentDirectory + "Data/Templates/" + Id);
    }

    public static DockerCustomTemplate? GetTemplateInfo(string id)
    {
        if (Program.CustomTemplates.TryGetValue(id, out DockerCustomTemplate? template))
            return template;
        return null;
    }

    public static DockerCustomTemplateData? GetTemplate(string? id)
    {
        if (string.IsNullOrEmpty(id))
            throw new Exception("Template id is missing.");

        if (Program.CustomTemplates.TryGetValue(id, out DockerCustomTemplate? template))
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

    public static string GetTemplateData(string? id)
    {
        if (string.IsNullOrEmpty(id))
            throw new Exception("Template id is missing");

        return File.ReadAllText(Program.CurrentDirectory + "Data/Templates/" + id + "/docker-compose.yml");
    }

    public static void ImportPortainerTemplates()
    {
        foreach (string i in Directory.GetDirectories("/var/lib/docker/volumes/portainer_data/_data/custom_templates"))
        {
            if (File.Exists(i + "/docker-compose.yml"))
            {
                string Data = File.ReadAllText(i + "/docker-compose.yml");
                string Id = Guid.NewGuid().ToString().Replace("-", "");

                IDeserializer deserializer = new DeserializerBuilder().Build();
                object? yamlObject = deserializer.Deserialize(Data);

                ISerializer serializer = new SerializerBuilder()
                    .JsonCompatible()
                    .Build();

                string json = serializer.Serialize(yamlObject);
                JObject JO = JObject.Parse(json);
                string? Name = null;
                try
                {
                    Name = JO["services"]?.First().Path.Replace("services.", "");
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

    public static void EditTemplate(string? id, EditCustomTemplateInfoEvent? data)
    {
        if (string.IsNullOrEmpty(id))
            throw new Exception("Template id is missing.");

        if (data == null)
            throw new Exception("Failed to parse template edit options.");

        if (Program.CustomTemplates.TryGetValue(id, out DockerCustomTemplate? template))
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

    public static void EditTemplateData(string? id, EditCustomTemplateComposeEvent? data)
    {
        if (string.IsNullOrEmpty(id))
            throw new Exception("Template id is missing.");

        if (data == null)
            throw new Exception("Failed to parse template edit options.");

        File.WriteAllText(Program.CurrentDirectory + "Data/Templates/" + id + "/docker-compose.yml", data.Data);
    }

    public static void DeleteTemplate(string? id)
    {
        if (string.IsNullOrEmpty(id))
            throw new Exception("Template id is missing.");

        Program.CustomTemplates.Remove(id);
        Program.SaveTemplates();
        Directory.Delete(Program.CurrentDirectory + "Data/Templates/" + id);
    }

    public static object? ControlTemplate(DockerEvent @event)
    {
        switch (@event.CustomTemplateType)
        {
            case ControlCustomTemplateType.ViewInfo:
                return GetTemplate(@event.ResourceId);
            case ControlCustomTemplateType.ComposeInfo:
                return GetTemplateData(@event.ResourceId);
            case ControlCustomTemplateType.ViewFull:
                return GetTemplate(@event.ResourceId);
            case ControlCustomTemplateType.EditInfo:
                EditTemplate(@event.ResourceId, @event.Data?.ToObject<EditCustomTemplateInfoEvent>());
                break;
            case ControlCustomTemplateType.EditCompose:
                EditTemplateData(@event.ResourceId, @event.Data?.ToObject<EditCustomTemplateComposeEvent>());
                break;
            case ControlCustomTemplateType.Delete:
                DeleteTemplate(@event.ResourceId);
                break;
        }

        return null;
    }
}
