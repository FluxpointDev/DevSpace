namespace DevSpaceWeb.Components.DynamicForm.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class SelectOptionAttribute : Attribute
{
    public SelectOptionAttribute(string value, string name)
    {
        Value = value;
        Name = name;
    }

    public string Value { get; private set; }
    public string Name { get; private set; }
}
