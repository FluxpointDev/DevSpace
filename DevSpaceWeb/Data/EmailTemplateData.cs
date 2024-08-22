using DevSpaceWeb.Components.Markdown;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DevSpaceWeb.Data;

public class EmailTemplateData
{
    [BsonId]
    public ObjectId Id { get; set; }

    public EmailTemplateType Type { get; set; }

    public string GetTypeName() => GetTypeName(Type);

    public static string GetTypeName(EmailTemplateType type)
    {
        switch (type)
        {
            case EmailTemplateType.Header:
                return "Header";
            case EmailTemplateType.Footer:
                return "Footer";
            case EmailTemplateType.AccountConfirm:
                return "Account Confirm";
            case EmailTemplateType.AccountInvited:
                return "Account Invited";
            case EmailTemplateType.AccountDeleted:
                return "Account Deleted";
            case EmailTemplateType.JoinTeamRequest:
                return "Join Team Request";
            case EmailTemplateType.AccountPasswordChangeRequest:
                return "Password Change Request";
            case EmailTemplateType.AccountPasswordChanged:
                return "Password Changed";
            case EmailTemplateType.Account2FAEnabled:
                return "2FA Enabled";
            case EmailTemplateType.Account2FADisabled:
                return "2FA Disabled";
            case EmailTemplateType.AccountEnabled:
                return "Account Enabled";
            case EmailTemplateType.AccountDisabled:
                return "Account Disabled";
            case EmailTemplateType.AccessCode:
                return "Access Code";
            case EmailTemplateType.AccountEmailChangeCode:
                return "Email Change Code";
            case EmailTemplateType.AccountEmailChangeRequest:
                return "Email Change Request";
            case EmailTemplateType.AccountEmailChanged:
                return "Email Changed";
            case EmailTemplateType.NewSessionIp:
                return "New Session IP";
        }

        return "Error!";
    }

    public string GetTypeIcon() => GetTypeIcon(Type);

    public static string GetTypeIcon(EmailTemplateType type)
    {
        return "";
        //switch (type)
        //{
        //    case EmailTemplateType.Header:
        //        return Icons.Material.Filled.FeaturedPlayList;
        //    case EmailTemplateType.Footer:
        //        return Icons.Material.Filled.CallToAction;
        //    case EmailTemplateType.AccountConfirm:
        //        return Icons.Material.Filled.Check;
        //    case EmailTemplateType.AccountInvited:
        //        return Icons.Material.Filled.PersonAdd;
        //    case EmailTemplateType.AccountDeleted:
        //        return Icons.Material.Filled.DeleteForever;
        //    case EmailTemplateType.JoinTeamRequest:
        //        return Icons.Material.Filled.DomainAdd;
        //    case EmailTemplateType.AccountPasswordChangeRequest:
        //        return Icons.Material.Filled.MoveToInbox;
        //    case EmailTemplateType.AccountPasswordChanged:
        //        return Icons.Material.Filled.Password;
        //    case EmailTemplateType.Account2FAEnabled:
        //        return Icons.Material.Filled.VpnKey;
        //    case EmailTemplateType.Account2FADisabled:
        //        return Icons.Material.Filled.VpnKeyOff;
        //    case EmailTemplateType.AccountEnabled:
        //        return Icons.Material.Filled.LockOpen;
        //    case EmailTemplateType.AccountDisabled:
        //        return Icons.Material.Filled.Lock;
        //    case EmailTemplateType.AccessCode:
        //        return Icons.Material.Filled.Password;
        //    case EmailTemplateType.AccountEmailChangeCode:
        //        return Icons.Material.Filled.ForwardToInbox;
        //    case EmailTemplateType.AccountEmailChangeRequest:
        //        return Icons.Material.Filled.MarkEmailUnread;
        //    case EmailTemplateType.AccountEmailChanged:
        //        return Icons.Material.Filled.MarkEmailRead;
        //    case EmailTemplateType.NewSessionIp:
        //        return Icons.Material.Filled.LocationOn;
        //}

        //return Icons.Material.Filled.QuestionMark;
    }

    public string Name { get; set; }

    public string Body { get; set; }

    public bool IsCustomHtml { get; set; }

    public bool IsDisabled { get; set; }

    public async Task UpdateAsync()
    {
        var update = Builders<EmailTemplateData>.Update.Set(r => r, this);
        var filter = Builders<EmailTemplateData>.Filter.Eq(r => r.Id, Id);
        await _DB.EmailTemplates.Collection.UpdateOneAsync(filter, update);
    }

    public string ParseMarkdown(AuthUser user, string action, string other_email = "", string code = "", string reason = "", string ip = "", string country = "")
    {
        string TaggedBody = ParseTags(user, action, other_email, code, reason, ip, country);
        if (Type != EmailTemplateType.Header && Type != EmailTemplateType.Footer && !string.IsNullOrEmpty(_Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Header).Body))
            TaggedBody = _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Header).ParseTags(user, action, other_email, code, reason, ip, country) + "\n--- --- ---\n" + TaggedBody;

        if (Type != EmailTemplateType.Header && Type != EmailTemplateType.Footer && !string.IsNullOrEmpty(_Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Footer).Body))
            TaggedBody = TaggedBody + "\n--- --- ---\n" + _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Footer).ParseTags(user, action, other_email, code, reason, ip, country);

        return TaggedBody;
    }

    public string ParseHtml(AuthUser user, string action, string other_email = "", string code = "", string reason = "", string ip = "", string country = "")
    {
        string TaggedBody = ParseTags(user, action, other_email, code, reason, ip, country);
        if (Type != EmailTemplateType.Header && Type != EmailTemplateType.Footer && !string.IsNullOrEmpty(_Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Header).Body))
            TaggedBody = _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Header).ParseTags(user, action, other_email, code, reason, ip, country) + "\n&nbsp;" + TaggedBody;

        if (Type != EmailTemplateType.Header && Type != EmailTemplateType.Footer && !string.IsNullOrEmpty(_Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Footer).Body))
            TaggedBody = TaggedBody + "\n&nbsp;\n" + _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Footer).ParseTags(user, action, other_email, code, reason, ip, country);

        string Html = Markdown.BuildHtmlFromMarkdown(TaggedBody);

        string Style = "<head><style>" +
            @"
            .format-center {
                text-align: center;
            }
            " +
            "</style></head>";

        Html = Style + "<body style=\"max-width: 700px; margin-left: auto; margin-right: auto;\">" + Html + "</body>";
        Console.WriteLine("Got Html");
        Console.WriteLine("--- --- --- --- ---");
        Console.WriteLine(Html);
        Console.WriteLine("--- --- --- --- ---");
        return Html;
    }

    public string ParseTags(AuthUser user, string action, string other_email, string code, string reason, string ip, string country)
    {
        return Body.Replace("{user.name}", user.UserName).Replace("{user.email}", user.Email)
            .Replace("{instance.email}", !string.IsNullOrEmpty(_Data.Config.Instance.Email) ? "mailto:" + _Data.Config.Instance.Email : "Invalid Email")
            .Replace("{instance.name}", _Data.Config.Instance.Name).Replace("{action}", action).Replace("{other_email}", other_email)
            .Replace("{instance.icon}", _Data.Config.Instance.GetIconOrDefault(usePng: true)).Replace("{code}", code)
            .Replace("{ip}", ip).Replace("{country}", country)
            .Replace("{reason}", reason).Replace("{user.avatar}", user.GetAvatarOrDefault(usePng: true));
    }
}
public enum EmailTemplateType
{
    Header,
    Footer,

    // Confirm your account email address
    AccountConfirm,
    // You have been invited to create an account
    AccountInvited,
    // You have 
    JoinTeamRequest,
    // You have deleted your account
    AccountDeleted,

    // You have requested to change your password
    AccountPasswordChangeRequest,
    // Your account password has changed
    AccountPasswordChanged,

    // Your account 2FA has been enabled
    Account2FAEnabled,
    // Your account 2FA has been disabled!
    Account2FADisabled,

    // Your account has been enabled
    AccountEnabled,
    // Your account has been disabled
    AccountDisabled,

    // You have requested an email code for
    AccessCode,
    // A new IP has been registered 
    NewSessionIp,

    // You have requested to change your email
    AccountEmailChangeRequest,
    AccountEmailChangeCode,
    // You have changed your account email
    AccountEmailChanged
}
