using DevSpaceWeb.Apps.Data;

namespace DevSpaceWeb.API.Apps;

public class AppJson
{
    public AppJson(AppData data)
    {
        id = data.Id.ToString();
        name = data.Name.ToString();
        description = data.Description;
        owner_id = data.OwnerId.ToString();
        vanity_url = data.VanityUrl;
        created_at = data.CreatedAt;
        avatar_url = data.AvatarUrl;
        bot_id = data.BotId;
        bot_owner_id = data.BotOwnerId;
        bot_developer_ids = data.BotDeveloperIds;
        type = data.AppType;
        terms_of_service_url = data.TermsOfServiceUrl;
        privacy_policy_url = data.PrivacyPolicyUrl;
    }

    public string id { get; set; }
    public string name { get; set; }
    public string? description { get; set; }
    public string owner_id { get; set; }
    public string? vanity_url { get; set; }
    public DateTime created_at { get; set; }
    public string? avatar_url { get; set; }
    public string bot_id { get; set; }
    public string bot_owner_id { get; set; }
    public string[] bot_developer_ids { get; set; }
    public AppType type { get; set; }
    public string? terms_of_service_url { get; set; }
    public string? privacy_policy_url { get; set; }
}
