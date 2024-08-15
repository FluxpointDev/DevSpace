using DevSpaceWeb.Data;
using DevSpaceWeb.Database;
using MailKit.Net.Smtp;
using MimeKit;

namespace DevSpaceWeb.Services;

public class EmailService
{
    private HttpClient? ManagedEmailSystem;
    public async Task<SmtpClient> CreateSmtpAsync()
    {
        var Client = new SmtpClient();
        await Client.ConnectAsync(_Data.Config.Email.SmtpHost, _Data.Config.Email.SmtpPort);
        await Client.AuthenticateAsync(_Data.Config.Email.SmtpUser, _Data.Config.Email.SmtpPassword);
        return Client;

    }

    public bool CanSendEmail()
    {
        return ManagedEmailSystem != null;
    }

    public Task<bool> Send(SendMailType type, AuthUser user, string url = "", string code = "") => Send(type, user.UserName, user.Email, url, code);

    public async Task<bool> Send(SendMailType type, string name, string email, string url = "", string code = "")
    {
        if (string.IsNullOrEmpty(url))
            url = _Data.Config.Instance.PublicUrl;

        if (_Data.Config.Email.Type == ConfigEmailType.FluxpointManaged && !string.IsNullOrEmpty(_Data.Config.Email.ManagedEmailToken))
        {
            if (ManagedEmailSystem == null)
                ManagedEmailSystem = new HttpClient();

            var message = new HttpRequestMessage(HttpMethod.Get, "https://devspacesmtp.fluxpoint.dev/send")
            {
                Content = JsonContent.Create(new SendMailJson
                {
                    type = type,
                    name = name,
                    email = email,
                    instance = _Data.Config.Instance.Name,
                    url = url,
                    code = code
                })
            };
            message.Headers.Add("Authorization", _Data.Config.Email.ManagedEmailToken);
            var Res = await ManagedEmailSystem.SendAsync(message);

            return Res.IsSuccessStatusCode;
        }
        else
        {
            if (string.IsNullOrEmpty(_Data.Config.Email.SmtpHost))
                return false;


            try
            {
                using (var smtp = await CreateSmtpAsync())
                {
                    await smtp.ConnectAsync(_Data.Config.Email.SmtpHost, _Data.Config.Email.SmtpPort);
                    await smtp.AuthenticateAsync(_Data.Config.Email.SmtpUser, _Data.Config.Email.SmtpPassword);
                    await SendTemplate(smtp, type, name, email, url, code);
                    await smtp.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

    public async Task SendTemplate(SmtpClient client, SendMailType type, string name, string email, string url = "", string code = "")
    {
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress(_Data.Config.Instance.Name, _Data.Config.Email.SmtpUser));
        message.To.Add(new MailboxAddress(name, email));

        string Body = "";
        switch (type)
        {
            case SendMailType.Test:
                message.Subject = $"Test Email - {name} | {_Data.Config.Instance.Name}";
                Body = $"This is a test message from {_Data.Config.Instance.Name}.";
                break;
            case SendMailType.AccountConfirm:
                message.Subject = $"Confirm Account - {name} | {_Data.Config.Instance.Name}";
                Body = $"Click here to confirm your account for {email}" +
                    $"<br />" +
                    $"<a href=\"{url}\">{url}</a>";
                break;
            case SendMailType.AccountInvited:
                message.Subject = $"Invited | {_Data.Config.Instance.Name}";
                Body = $"Click here to create an account for {_Data.Config.Instance.Name} using {email}" +
                    $"<br />" +
                    $"<a href=\"{url}\">{url}</a>";
                break;
            case SendMailType.AccountPasswordChanged:
                message.Subject = $"Password Changed - {name} | {_Data.Config.Instance.Name}";
                Body = $"Your password for {email} has been changed." +
                    $"<br />" +
                    $"<a href=\"{url}\">{url}</a>";
                break;
            case SendMailType.AccountDeleted:
                message.Subject = $"Password Deleted - {name} | {_Data.Config.Instance.Name}";
                Body = $"Your account {email} has been deleted." +
                    $"<br />" +
                    $"<a href=\"{url}\">{url}</a>";
                break;
            case SendMailType.Account2FAEnabled:
                message.Subject = $"2FA Enabled - {name} | {_Data.Config.Instance.Name}";
                Body = $"Your account {email} Two-factor authentication has been enabled." +
                    $"<br />" +
                    $"<a href=\"{url}\">{url}</a>";
                break;
            case SendMailType.Account2FADisabled:
                message.Subject = $"2FA Disabled - {name} | {_Data.Config.Instance.Name}";
                Body = $"Your account {email} Two-factor authentication has been disabled." +
                    $"<br />" +
                    $"<a href=\"{url}\">{url}</a>";
                break;
            case SendMailType.AccountEnabled:
                message.Subject = $"Account Activated - {name} | {_Data.Config.Instance.Name}";
                Body = $"Your account {email} has been activated." +
                    $"<br />" +
                    $"<a href=\"{url}\">{url}</a>";
                break;
            case SendMailType.AccountDisabled:
                message.Subject = $"Account Deactivated - {name} | {_Data.Config.Instance.Name}";
                Body = $"Your account {email} has been deactivated." +
                    $"<br />" +
                    $"<a href=\"{url}\">{url}</a>";
                break;
            case SendMailType.AccountPasswordChangeConfirm:
                message.Subject = $"Account Password Change Request - {name} | {_Data.Config.Instance.Name}";
                Body = $"You have requested to change your password for {email} on {_Data.Config.Instance.Name}." +
                    $"<br />" +
                    $"Click here to change your password.\n<a href=\"{url}\">{url}</a>";
                break;
            case SendMailType.AccountPasswordChangeCode:
                message.Subject = $"Account Password Change Code - {name} | {_Data.Config.Instance.Name}";
                Body = $"You have requested to change your password for {email} on {_Data.Config.Instance.Name}." +
                    $"<br />" +
                    $"Enter the code {code} here.\n<a href=\"{url}\">{url}</a>";
                break;
            //case SendMailType.AccountEmailChanged:
            //    message.Subject = $"Account Email Changed - {name} | {_Data.Config.Instance.Name}";
            //    Body = $"Your account {name}'s email on {_Data.Config.Instance.Name} has been changed to {email_other}." +
            //        $"<br />" +
            //        $"<a href=\"{url}\">{url}</a>";
            //    break;
            //case SendMailType.AccountEmailChangeConfirm:
            //    message.Subject = $"Account Email Change Request - {name} | {_Data.Config.Instance.Name}";
            //    Body = $"You have requested to change your account email from {email} to {email_other} with the account {name}." +
            //        $"<br />" +
            //        $"Click here to confirm this change." +
            //        $"<br />" +
            //        $"<a href=\"{url}\">{url}</a>";
            //    break;
            case SendMailType.AccessCode:
                message.Subject = $"Access Code Request - {name} | {_Data.Config.Instance.Name}";
                Body = $"You have requested access to a protected page/resource with verification." +
                    $"<br />" +
                    $"Use the code {code}";
                break;
        }
        Body += "<br /><br /><br />" +
                $"Need Support?" +
                $"<br />" +
                $"You can email us at support@fluxpoint.dev.";

        message.Body = new TextPart("html")
        {
            Text = Body
        };

        await client.SendAsync(message);
    }

    public class SendMailJson
    {
        public string name { get; set; }
        public string email { get; set; }
        public string email_other { get; set; }
        public string code { get; set; }
        public string instance { get; set; }
        public string url { get; set; }
        public SendMailType type { get; set; }
    }
}
public enum SendMailType
{
    None, Test, AccountConfirm, AccountInvited, AccountDeleted,
    AccountPasswordChangeConfirm, AccountPasswordChangeCode, AccountPasswordChanged, AccountEmailChangeConfirm, AccountEmailChanged,
    Account2FAEnabled, Account2FADisabled, AccountEnabled, AccountDisabled, AccessCode
}
