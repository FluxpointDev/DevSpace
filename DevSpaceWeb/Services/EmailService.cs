using DevSpaceWeb.Data;
using DevSpaceWeb.Database;
using MailKit.Net.Smtp;

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

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

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
