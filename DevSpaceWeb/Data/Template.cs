namespace DevSpaceWeb.Data;

public class Template
{
    public string? Name;
    public string? Description;

    public string GetIcon()
    {
        return "https://cdn.fluxpoint.dev/devspace/default_avatar.webp";
    }
}
