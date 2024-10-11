using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DevSpaceWeb.Data;

public class DynamicFormItem
{
    public DynamicFormItem(object model, PropertyInfo prop)
    {
        TypeName = prop.Name;
        FieldName = prop.Name;
        var displayName = prop.GetCustomAttribute<DisplayAttribute>()?.GetName();
        if (!string.IsNullOrEmpty(displayName))
            FieldName = displayName;

        Value = prop.GetValue(model);
        switch (prop.PropertyType.Name)
        {
            case nameof(Int64):
                Type = DynamicFormType.Number;
                break;
            case nameof(String):
                Type = DynamicFormType.Text;
                break;
        }

        Property = prop;
    }

    public PropertyInfo Property { get; set; }

    public string TypeName { get; set; }

    public string FieldName { get; set; }
    public DynamicFormType Type { get; set; }

    public dynamic Value { get; set; }

    public void ValueChanged(object value)
    {

    }

    public string ErrorMessage { get; set; }

    public bool Validate()
    {
        var Context = new ValidationContext(Property.GetType());
        foreach (var i in Property.GetCustomAttributes<ValidationAttribute>())
        {
            var Validate = i.GetValidationResult(Value, Context);
            if (Validate != null && !string.IsNullOrEmpty(Validate.ErrorMessage))
            {
                ErrorMessage = Validate.ErrorMessage;
                return false;
            }
        }

        return true;
    }
}
public enum DynamicFormType
{
    Text, Multiline, Number
}
