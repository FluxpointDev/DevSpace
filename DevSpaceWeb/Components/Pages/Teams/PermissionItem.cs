namespace DevSpaceWeb.Components.Pages.Teams;

public class PermissionItem<T> where T : Enum
{
    public PermissionItem(T flag, string name, string description)
    {
        Flag = flag;
        Name = name;
        Description = description;
    }

    public T Flag { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
