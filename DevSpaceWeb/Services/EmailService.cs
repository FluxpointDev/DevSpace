using DevSpaceWeb.Data;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Services.Identity;
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
        if (_Data.Config.Email.Type == ConfigEmailType.FluxpointManaged)
            return !string.IsNullOrEmpty(_Data.Config.Email.ManagedEmailToken);
        else
            return !string.IsNullOrEmpty(_Data.Config.Email.SmtpHost);
    }

    public Task<bool> SendAccountConfirm(AuthUser user, string action)
        => Send(EmailTemplateType.AccountConfirm, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.AccountConfirm), action: action);

    public Task<bool> SendAccountInvited(AuthUser user, string action)
        => Send(EmailTemplateType.AccountInvited, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.AccountInvited), action: action);

    public Task<bool> SendAccountDeleted(AuthUser user)
        => Send(EmailTemplateType.AccountDeleted, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.AccountDeleted));

    public Task<bool> SendPasswordChangeRequest(AuthUser user, string action)
        => Send(EmailTemplateType.AccountPasswordChangeRequest, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.AccountPasswordChangeRequest), action: action);

    public Task<bool> SendPasswordChanged(AuthUser user, string action)
        => Send(EmailTemplateType.AccountPasswordChanged, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.AccountPasswordChanged), action: action);

    public Task<bool> Send2FAEnabled(AuthUser user)
        => Send(EmailTemplateType.Account2FAEnabled, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Account2FADisabled));

    public Task<bool> Send2FADisabled(AuthUser user)
        => Send(EmailTemplateType.Account2FADisabled, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.Account2FADisabled));

    public Task<bool> SendAccountEnabled(AuthUser user, string reason)
        => Send(EmailTemplateType.AccountEnabled, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.AccountEnabled), reason: reason);

    public Task<bool> SendAccountDisabled(AuthUser user, string reason)
        => Send(EmailTemplateType.AccountDisabled, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.AccountDisabled), reason: reason);

    public Task<bool> SendAccessCode(AuthUser user, string reason, string code)
        => Send(EmailTemplateType.AccessCode, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.AccessCode), code: code, reason: reason);

    public Task<bool> SendEmailChangeCode(AuthUser user, string other_email, string code)
        => Send(EmailTemplateType.AccountEmailChangeCode, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.AccountEmailChangeCode), code: code, other_email: other_email);

    public Task<bool> SendEmailChangeRequest(AuthUser user, string action)
        => Send(EmailTemplateType.AccountEmailChangeRequest, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.AccountEmailChangeRequest), action: action);

    public Task<bool> SendEmailChanged(AuthUser user, string other_email)
        => Send(EmailTemplateType.AccountEmailChanged, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.AccountEmailChanged), other_email: other_email);

    public Task<bool> SendNewSessionIP(AuthUser user, string ip, string country)
        => Send(EmailTemplateType.AccountInvited, user, _Data.Config.Email.GetActiveTemplateOrDefault(EmailTemplateType.NewSessionIp), ip: ip, country: country);

    public async Task<bool> Send(EmailTemplateType type, AuthUser user, EmailTemplateData? template, string body = "", string other_email = "", string action = "", string code = "", string reason = "", string ip = "", string country = "", string team_name = "")
    {
        if (Program.IsPreviewMode)
            return true;

        if (_Data.Config.Email.Type == ConfigEmailType.FluxpointManaged && !string.IsNullOrEmpty(_Data.Config.Email.ManagedEmailToken))
        {
            if (ManagedEmailSystem == null)
                ManagedEmailSystem = new HttpClient();

            var message = new HttpRequestMessage(HttpMethod.Get, "https://devspacesmtp.fluxpoint.dev/send")
            {
                Content = JsonContent.Create(new SendMailJson
                {
                    type = type,
                    name = user.UserName,
                    email = user.Email,
                    email_other = other_email,
                    instance_name = _Data.Config.Instance.Name,
                    instance_icon = _Data.Config.Instance.GetIconOrDefault(),
                    url = string.IsNullOrEmpty(action) ? _Data.Config.Instance.GetPublicUrl() : action,
                    code = code,
                    ip = ip,
                    country = country,
                    team_name = team_name,
                    reason = reason
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
                    if (template == null)
                    {
                        MimeMessage message = new MimeMessage();
                        message.From.Add(new MailboxAddress(_Data.Config.Instance.Name, _Data.Config.Email.SenderEmailAddress));
                        message.To.Add(new MailboxAddress(user.UserName, user.Email));
                        message.Subject = "Test Email";
                        message.Body = new TextPart("html")
                        {
                            Text = body
                        };

                        await smtp.SendAsync(message);
                    }
                    else
                        await SendTemplate(smtp, user, template, other_email, action, code, reason, ip, country, team_name);
                    await smtp.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

    }

    private async Task SendTemplate(SmtpClient client, AuthUser user, EmailTemplateData template, string other_email, string action = "", string code = "", string reason = "", string ip = "", string country = "", string team_name = "")
    {
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress(_Data.Config.Instance.Name, _Data.Config.Email.SmtpUser));
        message.To.Add(new MailboxAddress(user.UserName, user.Email));
        message.Subject = $"{template.GetTypeName()} - {user.UserName} | {_Data.Config.Instance.Name}";

        string Body = template.ParseHtml(user, action, other_email, code, reason, ip, country);

        message.Body = new TextPart("html")
        {
            Text = Body
        };

        await client.SendAsync(message);
    }

    public string RandomCodeGenerator(bool longCode)
    {
        string[] page = new string[longCode ? 8 : 6];
        Random rnd = new Random();
        for (int i = 0; i < page.Length; ++i)
        {
            int Number = rnd.Next(10);
            page[i] = Number.ToString();
            Task.Delay(TimeSpan.FromMilliseconds(Number));
        }

        return string.Join("", page);
    }


    public class SendMailJson
    {
        public string name { get; set; }
        public string email { get; set; }
        public string email_other { get; set; }
        public string code { get; set; }
        public string instance_name { get; set; }
        public string instance_icon { get; set; }
        public string team_name { get; set; }
        public string reason { get; set; }
        public string url { get; set; }
        public string ip { get; set; }
        public string country { get; set; }
        public EmailTemplateType type { get; set; }
    }
}
