using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Validation;

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
                HasCapital = char.IsLower(c);
            else if (!HasLower)
                HasLower = char.IsUpper(c);
            else if (!HasDigit)
                HasDigit = char.IsDigit(c);
        }
        return HasCapital && HasLower && HasDigit;
    }
}
