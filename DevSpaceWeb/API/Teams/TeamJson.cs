using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.API.Teams;

public class TeamJson : Response
{
    public TeamJson(TeamData data) : base(200)
    {
        id = data.Id.ToString();
        name = data.Name;
        owner_id = data.OwnerId.ToString();
        vanity_url = data.VanityUrl;
        if (data.ResourceId.HasValue)
            resource_id = data.ResourceId.Value.ToString();
        if (data.IconId.HasValue)
            icon_id = data.IconId.Value.ToString();

    }

    public string id { get; set; }
    public string name { get; set; }
    public string owner_id { get; set; }
    public string vanity_url { get; set; }
    public string resource_id { get; set; }
    public string icon_id { get; set; }
}
