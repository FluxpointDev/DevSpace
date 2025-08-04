using DevSpaceWeb.Components.DynamicForm.Attributes;
using DevSpaceWeb.Models.Defaults;

namespace DevSpaceWeb.Models.Account;

public class AccountChangeEmailModel : EmailModel
{
    [Placeholder("000000")]
    public string? Code { get; set; }
}
