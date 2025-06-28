using DevSpaceWeb.Components.Markdown;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using MongoDB.Driver;

namespace DevSpaceWeb.Data.Teams;

public class EmailTemplateData : IObject
{
    public EmailTemplateType Type { get; set; }

    public string GetTypeName() => GetTypeName(Type);

    public static string GetTypeName(EmailTemplateType type)
    {
        switch (type)
        {
            case EmailTemplateType.Test:
                return "Test";
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
        switch (type)
        {
            case EmailTemplateType.Test:
                return "question_mark";
            case EmailTemplateType.Header:
                return "featured_play_list";
            case EmailTemplateType.Footer:
                return "call_to_action";
            case EmailTemplateType.AccountConfirm:
                return "check";
            case EmailTemplateType.AccountInvited:
                return "person_add";
            case EmailTemplateType.AccountDeleted:
                return "delete_forever";
            case EmailTemplateType.JoinTeamRequest:
                return "domain_add";
            case EmailTemplateType.AccountPasswordChangeRequest:
                return "move_to_inbox";
            case EmailTemplateType.AccountPasswordChanged:
                return "password";
            case EmailTemplateType.Account2FAEnabled:
                return "vpn_key";
            case EmailTemplateType.Account2FADisabled:
                return "vpn_key_off";
            case EmailTemplateType.AccountEnabled:
                return "lock_open";
            case EmailTemplateType.AccountDisabled:
                return "lock";
            case EmailTemplateType.AccessCode:
                return "password";
            case EmailTemplateType.AccountEmailChangeCode:
                return "forward_to_inbox";
            case EmailTemplateType.AccountEmailChangeRequest:
                return "mark_email_unread";
            case EmailTemplateType.AccountEmailChanged:
                return "mark_email_read";
            case EmailTemplateType.NewSessionIp:
                return "location_on";
        }

        return "question_mark";
    }

    public string? Name { get; set; }

    public string? Body { get; set; }

    public bool IsCustomHtml { get; set; }

    public bool IsDisabled { get; set; }

    public async Task UpdateAsync()
    {
        UpdateDefinition<EmailTemplateData> update = Builders<EmailTemplateData>.Update.Set(r => r, this);
        FilterDefinition<EmailTemplateData> filter = Builders<EmailTemplateData>.Filter.Eq(r => r.Id, Id);
        await _DB.EmailTemplates.Collection.UpdateOneAsync(filter, update);
    }

    public string? ParseMarkdown(AuthUser user, string action, string other_email = "", string code = "", string reason = "", string ip = "", string country = "")
    {
        string? TaggedBody = ParseTags(user, action, other_email, code, reason, ip, country);
        if (string.IsNullOrEmpty(TaggedBody))
            return TaggedBody;

        if (Type != EmailTemplateType.Header && Type != EmailTemplateType.Footer && !string.IsNullOrEmpty(_Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Header).Body))
            TaggedBody = _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Header).ParseTags(user, action, other_email, code, reason, ip, country) + "\n--- --- ---\n" + TaggedBody;

        if (Type != EmailTemplateType.Header && Type != EmailTemplateType.Footer && !string.IsNullOrEmpty(_Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Footer).Body))
            TaggedBody = TaggedBody + "\n--- --- ---\n" + _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Footer).ParseTags(user, action, other_email, code, reason, ip, country);

        return TaggedBody;
    }

    public string ParseHtml(AuthUser user, string action, string other_email = "", string code = "", string reason = "", string ip = "", string country = "")
    {
        string? TaggedBody = ParseTags(user, action, other_email, code, reason, ip, country);
        if (!string.IsNullOrEmpty(TaggedBody))
        {
            if (Type != EmailTemplateType.Header && Type != EmailTemplateType.Footer && !string.IsNullOrEmpty(_Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Header).Body))
                TaggedBody = _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Header).ParseTags(user, action, other_email, code, reason, ip, country) + "\n&nbsp;" + TaggedBody;

            if (Type != EmailTemplateType.Header && Type != EmailTemplateType.Footer && !string.IsNullOrEmpty(_Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Footer).Body))
                TaggedBody = TaggedBody + "\n&nbsp;\n" + _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Footer).ParseTags(user, action, other_email, code, reason, ip, country);
        }

        string Html = Markdown.BuildHtmlFromMarkdown(TaggedBody);

        string Style = "<head><style>" +
            @"
            .format-center {
                text-align: center;
            }
            " +
            "</style></head>";

        Html = Style + "<body style=\"max-width: 700px; margin-left: auto; margin-right: auto;\">" + Html + "</body>";
        Logger.LogMessage("Got Html", LogSeverity.Debug);
        Logger.LogMessage("--- --- --- --- ---", LogSeverity.Debug);
        Logger.LogMessage(Html, LogSeverity.Debug);
        Logger.LogMessage("--- --- --- --- ---", LogSeverity.Debug);
        return Html;
    }

    public string? ParseTags(AuthUser user, string action, string other_email, string code, string reason, string ip, string country)
    {
        if (string.IsNullOrEmpty(Body))
            return Body;

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
    Test,
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
