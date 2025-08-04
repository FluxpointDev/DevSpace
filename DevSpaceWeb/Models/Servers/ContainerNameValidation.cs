using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Servers;

[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class ContainerNameValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string? actualValue = value as string;
        return Validate(actualValue);
    }

    public static bool Validate(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        if (value.Length < 2)
            return false;

        if (value.Length > 255)
            return false;

        bool IsCharactersValid = true;

        foreach (char c in value)
        {
            if (char.IsLower(c) || char.IsDigit(c))
                continue;

            if (c == '.' || c == '-' || c == '_')
                continue;

            IsCharactersValid = false;
        }
        return IsCharactersValid;
    }
}
