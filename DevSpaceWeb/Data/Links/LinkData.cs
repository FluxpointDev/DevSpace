using MongoDB.Bson;

namespace DevSpaceWeb.Data.Links;

public class LinkData
{
    public static LinkData Static = new LinkData();

    public string Title = "Hello World!";
    public string Description = "Test description here";
    public string BackgroundUrl;
    public bool RequireAuthentication;
    public bool RequireMember;
    public bool RequireRoles;
    public HashSet<ObjectId> RestrictedRoles = new HashSet<ObjectId>();
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