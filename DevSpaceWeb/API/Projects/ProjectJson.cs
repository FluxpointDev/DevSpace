using DevSpaceWeb.Data.Projects;

namespace DevSpaceWeb.API.Projects;

public class ProjectJson
{
    public ProjectJson(ProjectData data)
    {
        id = data.Id.ToString();
        name = data.Name.ToString();
        owner_id = data.OwnerId.ToString();
        vanity_url = data.VanityUrl;
        created_at = data.CreatedAt;
    }

    public string id { get; set; }
    public string name { get; set; }
    public string? description { get; set; }
    public string owner_id { get; set; }
    public string? vanity_url { get; set; }
    public DateTime created_at { get; set; }
}
