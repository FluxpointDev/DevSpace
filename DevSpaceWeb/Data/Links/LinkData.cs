using DevSpaceWeb.Data.Teams;
using MongoDB.Bson;

namespace DevSpaceWeb.Data.Links;

public class LinkData : ITeamResource
{
    public LinkData() : base(ResourceType.Link)
    {

    }

    public string ShortCode { get; set; }
    public string Link { get; set; }
}

public class LinkDataOld
{
    public static LinkDataOld Static = new LinkDataOld();

    public string Title = "Hello World!";
    public string Description = "Test description here";
    public string? BackgroundUrl;
    public bool RequireAuthentication;
    public bool RequireMember;
    public bool RequireRoles;
    public HashSet<ObjectId> RestrictedRoles = [];
    public int ButtonTimer = 3;
    public bool RequireCode;
    public string? Code;
    public string? Link;
    public bool IsDownload;

    public string GetDomain()
    {
        return Utils.GetDomain(Link)!;
    }
}