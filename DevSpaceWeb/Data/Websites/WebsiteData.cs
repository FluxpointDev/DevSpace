using DevSpaceWeb.Components.Layout;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;

namespace DevSpaceWeb.Data.Websites;

public class WebsiteData : ITeamResource
{
    public string Domain { get; set; }
    public bool HasAccess(SessionProvider session)
    {
        return true;
    }

    public string GetVanityUrl()
    {
        if (_DB.VanityUrlCache.TryGetValue(Id, out string vanityUrl))
            return vanityUrl;

        return Id.ToString();
    }
}
