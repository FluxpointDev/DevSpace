namespace DevSpaceWeb.Data;

public class FileResource
{
    public FileResource(string tag, Guid? resource, Guid? id)
    {
        Tag = tag;
        Resource = resource;
        Id = id;
    }

    private string Tag;

    public Guid? Resource { get; set; }

    public Guid? Id { get; set; }

    public string Path(string fileType)
    {
        return Program.Directory.Public.Resources.Path + Resource.ToString() + $"/{Tag}_" + Id.Value.ToString() + "." + fileType;
    }

    public string Url(string fileType)
    {
        return _Data.Config.Instance.GetPublicUrl() + "/public/resources/" + Resource.ToString() + $"/{Tag}_" + Id.ToString() + "." + fileType;
    }
}
