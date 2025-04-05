using DevSpaceWeb.Models.Defaults;

namespace DevSpaceWeb.Models.Account;

public class AccountChangeEmailModel : EmailModel
{
    public string? Code { get; set; }
}
