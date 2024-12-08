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
        bool HasSpecial = false;
        foreach (char c in actualValue)
        {
            if (!HasCapital)
                HasCapital = char.IsLower(c);
            else if (!HasLower)
                HasLower = char.IsUpper(c);
            else if (!HasDigit)
                HasDigit = char.IsDigit(c);
            else if (HasSpecial)
                HasSpecial = char.IsSymbol(c);
        }
        if (actualValue.Equals("password", StringComparison.OrdinalIgnoreCase))
            return false;

        int Strength = 0;
        if (HasCapital)
            Strength += 1;
        if (HasLower)
            Strength += 1;
        if (HasDigit)
            Strength += 1;
        if (HasSpecial)
            Strength += 1;

        return Strength >= 2;
    }
}
