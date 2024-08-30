using DevSpaceWeb.Models.Defaults;
using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Models.Validation;

[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class UsernameValidationAttribute : ValidationAttribute
{
    public char[] AllowableValues { get; set; } = UsernameModel.AllowedCharacters.ToCharArray();

    public override bool IsValid(object value)
    {
        string actualValue = value as string;

        foreach (var c in actualValue)
        {
            if (!AllowableValues.Contains(c))
                return false;
        }
        return true;
    }
}
