namespace DevSpaceWeb.Apps.Runtime;

public class FileData
{
    public string Mime { get; set; }
    public string Name { get; set; }

    public string GetFileName()
    {
        switch (Mime)
        {
            case "image/png":
                return (Name ?? "image") + ".png";
            case "image/jpg":
            case "image/jpeg":
                return (Name ?? "image") + ".jpg";
            case "image/webp":
                return (Name ?? "image") + ".webp";

        }
        return Name ?? "file";
    }

    public string Url { get; set; }

    public byte[]? Buffer { get; set; } = null;
}
