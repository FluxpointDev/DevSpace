using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Apps;

[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class AppSlashCommandNameValidationAttribute : ValidationAttribute
{
    public static char[] AllowedCharacters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

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
[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class AppCommandNameValidationAttribute : ValidationAttribute
{
    public static char[] AllowedCharacters = "abcdefghijklmnopqrstuvwxyz_-".ToCharArray();

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