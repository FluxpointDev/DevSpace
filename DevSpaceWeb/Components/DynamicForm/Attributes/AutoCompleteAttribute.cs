using System.ComponentModel.DataAnnotations;

namespace DevSpaceWeb.Components.DynamicForm.Attributes;


public class AutoCompleteAttribute : Attribute
{
    public AutoCompleteAttribute(AutoCompleteType type)
    {
        Type = type;
    }

    public AutoCompleteType Type { get; private set; }
}
public enum AutoCompleteType
{
    /// <summary> The person's email address. </summary>
    [Display(Name = "email")]
    Email,

    /// <summary> The person's full name. </summary>
    [Display(Name = "name")]
    FullName,

    /// <summary> The person's first/given name. </summary>
    [Display(Name = "given-name")]
    FirstName,

    /// <summary> The person's last name. </summary>
    [Display(Name = "family-name")]
    LastName,

    /// <summary> The person's username. </summary>
    [Display(Name = "username")]
    Username,

    /// <summary> The person's new password. </summary>
    [Display(Name = "new-password")]
    NewPassword,

    /// <summary> The person's current password. </summary>
    [Display(Name = "current-password")]
    CurrentPassword,

    /// <summary> The person's one time or 2FA code. </summary>
    [Display(Name = "one-time-code")]
    OneTimeCode,

    /// <summary> A website url. </summary>
    [Display(Name = "url")]
    Url,
}
