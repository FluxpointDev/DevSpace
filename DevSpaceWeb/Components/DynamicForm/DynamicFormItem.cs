﻿using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace DevSpaceWeb.Components.DynamicForm;

public class DynamicFormItem
{
    public DynamicFormItem(object model, PropertyInfo prop)
    {
        SetLabelText(prop);
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

    private string SetLabelText(PropertyInfo prop)
    {
        bool IsUppercase = false;
        StringBuilder str = new StringBuilder();
        foreach (char i in prop.Name)
        {
            if (Char.IsUpper(i))
            {
                if (!IsUppercase)
                {
                    str.Append(' ');
                    IsUppercase = true;
                }

            }
            else
            {
                if (IsUppercase)
                {
                    IsUppercase = false;
                }
            }
            str.Append(i);
        }
        Label = str.ToString();
        DisplayAttribute? DisplayName = prop.GetCustomAttribute<DisplayAttribute>();
        if (DisplayName != null && !string.IsNullOrEmpty(DisplayName.Name))
            Label = DisplayName.Name;
        return Label;
    }

    public PropertyInfo Property { get; set; }

    public string Label { get; set; }
    public DynamicFormType Type { get; set; }

    public string ErrorMessage { get; set; }

    public bool Validate(object model)
    {
        ValidationContext Context = new ValidationContext(Property.GetType());
        foreach (ValidationAttribute i in Property.GetCustomAttributes<ValidationAttribute>())
        {
            ValidationResult? Validate = i.GetValidationResult(Property.GetValue(model), Context);
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
