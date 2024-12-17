using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.API.Teams;

public class TeamJson : Response
{
    public TeamJson(TeamData data) : base(200)
    {
        id = data.Id.ToString();
        name = data.Name;
        ownerId = data.OwnerId.ToString();
        vanityUrl = data.VanityUrl;
        if (data.ResourceId.HasValue)
            resourceId = data.ResourceId.Value.ToString();
        if (data.IconId.HasValue)
            iconId = data.IconId.Value.ToString();

    }

    public string id { get; set; }
    public string name { get; set; }
    public string ownerId { get; set; }
    public string vanityUrl { get; set; }
    public string resourceId { get; set; }
    public string iconId { get; set; }
}
