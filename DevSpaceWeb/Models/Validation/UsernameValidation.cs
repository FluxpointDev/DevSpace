using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Validation;

[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class UsernameValidationAttribute : ValidationAttribute
{
    public static char[] AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._".ToCharArray();

    public override bool IsValid(object? value)
    {
        string? actualValue = value as string;
        if (string.IsNullOrEmpty(actualValue))
            return false;

        foreach (char c in actualValue)
        {
            if (!AllowedCharacters.Contains(c))
                return false;
        }
        return true;
    }
}
