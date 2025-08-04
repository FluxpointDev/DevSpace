namespace DevSpaceWeb.Components.DynamicForm.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class PlaceholderAttribute : Attribute
{
    public PlaceholderAttribute(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }
}
