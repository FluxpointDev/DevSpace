class ValidDiscordConnections extends ValidConnections {
    DataMessages = ["data_message_*", "variables_get"];
    OutputMessages = ["data_message_active", "variables_get"];

    DataChannels = ["data_channel_*", "variables_get"];
    OutputChannels = ["data_channel_active", "variables_get"];

    DataServers = ["data_server_*", "variables_get"];
    OutputServers = ["data_server_active", "variables_get"];

    DataCategory = ["data_category_*", "variables_get"];
    OutputCategory = ["data_category_active", "variables_get"];

    DataUsers = ["data_user_*", "data_member_*", "variables_get"];
    OutputUsers = ["data_user_active", "data_member_active", "variables_get"];

    DataMembers = ["data_member_*", "variables_get"];
    OutputMembers = ["data_member_active", "variables_get"];

    DataRoles = ["data_role_*", "variables_get"];
    OutputRoles = ["data_role_active", "variables_get"];

    DataWebhooks = ["data_webhook_*", "variables_get"];
    OutputWebhooks = ["data_webhook_active", "variables_get"];

    DataEmojis = ["data_emoji_*", "variables_get"];
    OutputWebhooks = ["data_emoji_active", "variables_get"];
    
    DataPermissions = ["data_permission_manage", "data_permission_mod", "data_permission_server", "data_permission_text", "data_permission_voice", "data_permission_list", 'variables_get'];

    DataCategory = ["data_category_*", "variables_get"];

    DataInteraction = ["variables_get", "data_emoji_active", "data_webhook_active", "data_role_active", "data_member_active", "data_user_active", "data_category_active", "data_channel_active", "data_server_active", "data_message_active"];
}