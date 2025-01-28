using DevSpaceWeb.Data.Teams;

namespace DevSpaceWeb.Data.Websites;

public class WebsiteData : ITeamResource
{
    public WebsiteData() : base(ResourceType.Website) { }

    public string Domain { get; set; }
}
