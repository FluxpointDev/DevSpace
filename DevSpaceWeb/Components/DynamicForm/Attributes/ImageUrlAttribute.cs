using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DevSpaceWeb.Components.DynamicForm.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ImageUrlAttribute : ValidationAttribute
{
    public ImageUrlAttribute(string[]? extensions = null)
    {
        if (string.IsNullOrEmpty(ErrorMessage))
            ErrorMessage = $"{0} is not a valid image url.";

        Extensions = extensions ?? DefaultExtensions;
    }

    public static string[] DefaultExtensions { get; set; } = new string[] {
        "png", "jpg", "jpeg", "webp", "avif", "gif",
    };
    public string[]? Extensions { get; private set; }

    public override bool IsValid(object value)
    {
        if (value == null)
        {
            return true;
        }

        if (value is not string str)
            return false;

        bool IsValid = (str.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || str.StartsWith("https://", StringComparison.OrdinalIgnoreCase));

        if (IsValid)
        {
            string[] Split = str.Split('?');
            foreach (string i in Extensions)
            {
                if (Split[0].EndsWith("." + i, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return String.Format(CultureInfo.CurrentCulture,
          ErrorMessageString, name);
    }
}