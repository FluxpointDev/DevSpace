using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models;

[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class PasswordValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        string actualValue = value as string;
        bool HasCapital = false;
        bool HasLower = false;
        bool HasDigit = false;

        foreach (var c in actualValue)
        {
            if (!HasCapital)
                HasCapital = Char.IsLower(c);
            else if (!HasLower)
                HasLower = Char.IsUpper(c);
            else if (!HasDigit)
                HasDigit = Char.IsDigit(c);
        }
        return HasCapital && HasLower && HasDigit;
    }
}
