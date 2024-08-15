using DevSpaceWeb.Components.Markdown;
using DevSpaceWeb.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevSpaceWeb.Data;

public class EmailTemplateData
{
    [BsonId]
    public ObjectId Id { get; set; }

    public EmailTemplateType Type { get; set; }

    public string Name { get; set; }

    public string Body { get; set; }

    public bool IsCustomHtml { get; set; }

    public bool IsDisabled { get; set; }

    public string ParseMarkdown(AuthUser user, string action, string other_email = "", string code = "", string purpose = "")
    {
        string TaggedBody = _Data.Config.Email.GetTemplate(EmailTemplateType.Header).ParseTags(user, action, other_email, code, purpose) + "\n" +
            ParseTags(user, action, other_email, code, purpose) +
            _Data.Config.Email.GetTemplate(EmailTemplateType.Footer).ParseTags(user, action, other_email, code, purpose)

        return Markdown.BuildHtmlFromMarkdown(TaggedBody);
    }

    public string ParseTags(AuthUser user, string action, string other_email = "", string code = "", string purpose = "")
    {
        return Body.Replace("{user.name}", user.UserName).Replace("{user.email}", user.Email)
            .Replace("{instance.name}", _Data.Config.Instance.Name).Replace("{action}", action).Replace("{email}", other_email)
            .Replace("{instance.icon}", _Data.Config.Instance.GetIconOrDefault()).Replace("{code}", code)
            .Replace("{purpose}", purpose).Replace("{user.avatar}", user.GetAvatarOrDefault());
    }
}
public enum EmailTemplateType
{
    Header,
    Footer,

    Test,
    // Confirm your account email address
    AccountConfirm,
    // You have been invited to create an account
    AccountInvited,
    // You have 
    AccountJoinTeam,
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

    // You have requested to change your email
    AccountEmailChangeRequest,
    AccountEmailChangeCode,
    // You have changed your account email
    AccountEmailChanged
}
