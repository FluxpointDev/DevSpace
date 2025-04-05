using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace DevSpaceWeb.Models.Validation;

[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class RequiredIfValidationAttribute : ValidationAttribute
{
    /// <summary>
    /// Gets or sets a value indicating whether other property's value should
    /// match or differ from provided other property's value (default is <c>false</c>).
    /// </summary>
    public bool IsInverted { get; set; } = false;

    /// <summary>
    /// Gets or sets the other property name that will be used during validation.
    /// </summary>
    /// <value>
    /// The other property name.
    /// </value>
    public string OtherProperty { get; private set; }

    /// <summary>
    /// Gets or sets the other property value that will be relevant for validation.
    /// </summary>
    /// <value>
    /// The other property value.
    /// </value>
    public object OtherPropertyValue { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredIfValidationAttribute"/> class.
    /// </summary>
    /// <param name="otherProperty">The other property.</param>
    /// <param name="otherPropertyValue">The other property value.</param>
    public RequiredIfValidationAttribute(string otherProperty, object otherPropertyValue)
        : base()
    {
        OtherProperty = otherProperty;
        OtherPropertyValue = otherPropertyValue;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {

        PropertyInfo? otherPropertyInfo = validationContext
            .ObjectType.GetProperty(OtherProperty);
        if (otherPropertyInfo == null)
        {
            return new ValidationResult(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "Could not find a property named {0}.",
                validationContext.ObjectType, OtherProperty));
        }

        // Determine whether to run [Required] validation
        object? actualOtherPropertyValue = otherPropertyInfo
            .GetValue(validationContext.ObjectInstance, null);
        if (!IsInverted && Equals(actualOtherPropertyValue, OtherPropertyValue) ||
            IsInverted && !Equals(actualOtherPropertyValue, OtherPropertyValue))
        {
            return base.IsValid(value, validationContext);
        }
        return default;
    }
}
