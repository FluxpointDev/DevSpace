namespace DevSpaceWeb.Data;

public static class EmailTemplateDefaults
{
    public static Dictionary<EmailTemplateType, string> List = new Dictionary<EmailTemplateType, string>
    {
        { EmailTemplateType.Header, "![Icon]({instance.icon}) {.format-center}\n## {instance.name} {.format-center}" },
        { EmailTemplateType.Footer, "Need support?\nContact us at {instance.email}" },
        { EmailTemplateType.AccountInvited, "You have been invited to create an account on **{instance.name}**\n\nCreate one here {action}" },
        { EmailTemplateType.JoinTeamRequest, "You have been invited to join the team **{team.name}** on **{instance.name}**\n\nClick here to accept {action}" },
        { EmailTemplateType.AccountConfirm, "You need to confirm your account **{user.name}** on **{instance.name}**\n\nClick here to [Confirm Account]({action})" },
        { EmailTemplateType.AccountDeleted, "Your account **{user.name}** has been deleted on **{instance.name}**" },

        { EmailTemplateType.AccountPasswordChangeRequest, "You have requested to change your password for **{user.name}**\n\nClick here to [Change Password]({action})" },
        { EmailTemplateType.AccountPasswordChanged, "Your account password for **{user.name}** has been changed.\n\nIf this was not you change your password at {action}" },

        { EmailTemplateType.Account2FAEnabled, "Your account **{user.name}** has been setup with 2FA (Two-factor) authentication." },
        { EmailTemplateType.Account2FADisabled, "2FA (Two-factor) authentication for your account **{user.name}** has been disabled!" },

        { EmailTemplateType.AccountEnabled, "Your account **{user.name}** has been activated on **{instance.name}**" },
        { EmailTemplateType.AccountDisabled, "Your account **{user.name}** has been disabled on **{instance.name}** with the reason {reason}" },

        { EmailTemplateType.AccessCode, "You have requested an email code for your account **{user.name}**\nThis was sent due to {reason}.\n\nUse the code **{code}** to confirm." },

        { EmailTemplateType.AccountEmailChangeCode, "An account on **{instance.name}** is requesting to be linked to this email address.\nYou will need to enter the code to confirm this change.\n\nUse the code **{code}** to confirm." },
        { EmailTemplateType.AccountEmailChangeRequest, "You have requested to change your account email address for **{user.name}**\nThe new email address will be **{other_email}**\n\nClick here to [Change Email Address]({action})" },
        { EmailTemplateType.AccountEmailChanged, "Your account **{user.name}** has been changed to the new email address **{other_email}**" },

        { EmailTemplateType.NewSessionIp, "Your account **{user.name}** has been logged in with a new IP address for **{ip}** in **{country}**" },
    };
}
