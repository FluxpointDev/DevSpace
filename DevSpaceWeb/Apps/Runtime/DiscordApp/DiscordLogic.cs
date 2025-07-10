using Discord;
using Discord.Rest;
using Newtonsoft.Json.Linq;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp;

public static class DiscordLogic
{
    public static async Task<bool> Parse(DiscordRuntime runtime, WorkspaceBlock block)
    {
        bool result = false;
        switch (block.type)
        {
            case "logic_condition_and":
                {
                    bool FullResult = false;
                    foreach (WorkspaceBlockConnection i in block.inputs.Values)
                    {
                        if (i.block == null)
                            continue;

                        string compare = string.Empty;
                        if (i.block.fields.TryGetValue("compare", out JToken compareJson))
                            compare = compareJson.ToString();

                        FullResult = await ParseBlock(runtime, i.block);

                        if (!FullResult)
                            break;
                    }
                    return FullResult;
                }
            case "logic_condition_or":
                {
                    foreach (WorkspaceBlockConnection i in block.inputs.Values)
                    {
                        if (i.block == null)
                            continue;

                        string compare = string.Empty;
                        if (i.block.fields.TryGetValue("compare", out JToken compareJson))
                            compare = compareJson.ToString();

                        if (await ParseBlock(runtime, i.block))
                            return true;

                    }
                }
                break;
            default:
                return await ParseBlock(runtime, block);
        }
        return result;
    }

    public static async Task<bool> ParseBlock(DiscordRuntime runtime, WorkspaceBlock block)
    {
        if (block.enabled)
        {
            string property = string.Empty;
            string compare = string.Empty;
            if (block.fields.TryGetValue("property", out JToken propJson))
                property = propJson.ToString();

            if (block.fields.TryGetValue("compare", out JToken compareJson))
                compare = compareJson.ToString();

            if (string.IsNullOrEmpty(property))
                throw new RuntimeError(RuntimeErrorType.Server, "Failed to check logic condition, property is missing.");

            if (string.IsNullOrEmpty(compare))
                throw new RuntimeError(RuntimeErrorType.Server, "Failed to check logic condition, compare is missing.");

            WorkspaceBlock? inputBlock = null;
            KeyValuePair<string, WorkspaceBlockConnection>? first = block.inputs.FirstOrDefault();
            if (first.HasValue && first.Value.Value.block != null && first.Value.Value.block.enabled)
                inputBlock = first.Value.Value.block;

            WorkspaceBlock? valueBlock = null;
            first = block.inputs.LastOrDefault();
            if (first.HasValue && first.Value.Value.block != null && first.Value.Value.block.enabled)
                valueBlock = first.Value.Value.block;

            if (inputBlock == null)
                throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, input block is missing.");


            //switch (property)
            //{
            //    case "":
            //        {
            //            switch (compare)
            //            {
            //                case "":
            //                    break;
            //            }
            //        }
            //        break;
            //}

            switch (block.type)
            {
                case "logic_check_basic":
                    {
                        string id = inputBlock.GetVariableId();
                        object? variable = runtime.Variables.GetValueOrDefault(id);
                        switch (property)
                        {
                            case "basic_type":
                                {
                                    switch (compare)
                                    {
                                        case "is_string":
                                            if (variable is string)
                                                return InvertIfValueBool(runtime, true, valueBlock);
                                            break;
                                        case "is_boolean":
                                            if (variable is bool)
                                                return InvertIfValueBool(runtime, true, valueBlock);
                                            break;
                                        case "is_number":
                                            if (variable is int || variable is double)
                                                return InvertIfValueBool(runtime, true, valueBlock);
                                            break;
                                        case "is_integer":
                                            if (variable is int)
                                                return InvertIfValueBool(runtime, true, valueBlock);
                                            break;
                                        case "is_double":
                                            return InvertIfValueBool(runtime, variable is double, valueBlock);
                                    }
                                    return InvertIfValueBool(runtime, false, valueBlock);
                                }
                                break;
                            case "data_type":
                                {
                                    switch (compare)
                                    {
                                        case "is_file":
                                            return InvertIfValueBool(runtime, variable is FileData, valueBlock); ;
                                        case "is_json":
                                            return InvertIfValueBool(runtime, variable is JObject, valueBlock); ;
                                        case "is_response":
                                            return InvertIfValueBool(runtime, variable is ResponseData, valueBlock);

                                    }

                                    return InvertIfValueBool(runtime, false, valueBlock);
                                }
                                break;
                            case "string":
                                if (compare == "exists")
                                    return await CheckString(runtime, variable.ToString(), compare, valueBlock);
                                else
                                {
                                    if (valueBlock == null)
                                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value block is missing.");

                                    return await CheckString(runtime, variable.ToString(), compare, valueBlock);
                                }
                            case "boolean":
                                return CheckBool(runtime, (bool)variable, valueBlock, compare);
                            case "number":
                                return await CheckNumber(runtime, inputBlock, valueBlock, compare);
                        }
                    }
                    break;
                case "logic_check_file":
                    {
                        switch (property)
                        {
                            case "file":
                                {
                                    switch (compare)
                                    {
                                        case "exists":
                                            {
                                                FileData? FileData = runtime.GetFileFromBlock(inputBlock);
                                                if (FileData != null && FileData.Buffer != null)
                                                    return InvertIfValueBool(runtime, true, valueBlock);
                                            }
                                            break;
                                        case "is_image":
                                            {
                                                FileData? FileData = runtime.GetFileFromBlock(inputBlock);
                                                if (FileData != null)
                                                    return InvertIfValueBool(runtime, FileData.Mime.StartsWith("image/"), valueBlock);
                                            }
                                            break;
                                    }

                                    return InvertIfValueBool(runtime, false, valueBlock);
                                }
                                break;
                            case "name":
                                {
                                    FileData? FileData = runtime.GetFileFromBlock(inputBlock);
                                    if (FileData != null && FileData.Buffer != null)
                                        return await CheckString(runtime, FileData.Name, compare, valueBlock);
                                }
                                break;
                            case "length":
                                {
                                    FileData? FileData = runtime.GetFileFromBlock(inputBlock);
                                    if (FileData != null && FileData.Buffer != null)
                                        return await CheckNumber(runtime, FileData.Buffer.Length, 0, valueBlock, compare);
                                }
                                break;
                            case "type":
                                {
                                    FileData? FileData = runtime.GetFileFromBlock(inputBlock);
                                    if (FileData != null && FileData.Buffer != null)
                                    {
                                        switch (compare)
                                        {
                                            case "is_png":
                                                return InvertIfValueBool(runtime, FileData.Mime == "image/png", valueBlock);
                                            case "is_jpg":
                                                return InvertIfValueBool(runtime, (FileData.Mime == "image/jpg" || FileData.Mime == "image/jpeg"), valueBlock);
                                            case "is_webp":
                                                return InvertIfValueBool(runtime, FileData.Mime == "image/webp", valueBlock);
                                        }
                                    }
                                }
                                return InvertIfValueBool(runtime, false, valueBlock);


                        }
                    }
                    break;
                case "logic_check_json":
                    {
                        switch (property)
                        {
                            case "json":
                                {
                                    switch (compare)
                                    {
                                        case "exists":
                                            JObject? json = runtime.GetJsonFromBlock(inputBlock);
                                            return InvertIfValueBool(runtime, json != null, valueBlock);
                                    }
                                }
                                break;
                            case "length":
                                return await CheckNumber(runtime, inputBlock, valueBlock, compare);
                            case "key":
                                {
                                    switch (compare)
                                    {
                                        case "has":
                                        case "not_has":
                                            {
                                                if (valueBlock == null)
                                                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value block is missing.");

                                                JObject? Json = runtime.GetJsonFromBlock(inputBlock);
                                                string Key = await runtime.GetStringFromBlock(valueBlock);
                                                if (Json != null)
                                                {
                                                    JToken? Token = runtime.SelectJsonToken(Json, Key);
                                                    if (Token != null)
                                                        return compare == "has";
                                                }
                                            }
                                            return compare != "has";
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case "logic_check_response":
                    switch (property)
                    {
                        case "response":
                            {
                                switch (compare)
                                {
                                    case "exists":
                                        {
                                            ResponseData? Res = runtime.GetResponseFromBlock(inputBlock);
                                            return InvertIfValueBool(runtime, Res != null, valueBlock);
                                        }

                                    case "is_success":
                                        {
                                            ResponseData? Res = runtime.GetResponseFromBlock(inputBlock);
                                            if (Res != null)
                                                return InvertIfValueBool(runtime, Res.IsSuccess, valueBlock);

                                            return InvertIfValueBool(runtime, false, valueBlock);
                                        }
                                        break;
                                    case "has_body":
                                        {
                                            ResponseData? Res = runtime.GetResponseFromBlock(inputBlock);
                                            if (Res != null)
                                                return InvertIfValueBool(runtime, Res.ContentLength != 0, valueBlock);

                                            return InvertIfValueBool(runtime, false, valueBlock);
                                        }
                                        break;
                                }

                                return InvertIfValueBool(runtime, false, valueBlock);
                            }
                            break;
                        case "status_code":
                            {
                                ResponseData? Res = runtime.GetResponseFromBlock(inputBlock);
                                if (Res != null)
                                    return await CheckNumber(runtime, Res.StatusCode, 0, valueBlock, compare);
                            }
                            break;
                        case "body_length":
                            {
                                ResponseData? Res = runtime.GetResponseFromBlock(inputBlock);
                                if (Res != null)
                                    return await CheckNumber(runtime, 0, Res.ContentLength, valueBlock, compare);
                            }
                            break;
                        case "body_type":
                            {
                                ResponseData? Res = runtime.GetResponseFromBlock(inputBlock);
                                if (Res != null)
                                    return await CheckString(runtime, Res.ContentType, compare, valueBlock);
                            }
                            break;
                        case "headers":
                            switch (compare)
                            {
                                case "contains":
                                    {
                                        if (valueBlock == null)
                                            throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value block is missing.");

                                        ResponseData? Res = runtime.GetResponseFromBlock(inputBlock);
                                        if (Res != null)
                                            return Res.Headers.Contains(await runtime.GetStringFromBlock(valueBlock));
                                    }
                                    break;
                                case "not_contains":
                                    {
                                        if (valueBlock == null)
                                            throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value block is missing.");

                                        ResponseData? Res = runtime.GetResponseFromBlock(inputBlock);
                                        if (Res != null)
                                            return !Res.Headers.Contains(await runtime.GetStringFromBlock(valueBlock));
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case "logic_check_discord_variable":
                    {
                        switch (property)
                        {
                            case "data_type":
                                {
                                    Type? obj = null;

                                    if (obj == null)
                                        return InvertIfValueBool(runtime, false, valueBlock);

                                    switch (inputBlock.type)
                                    {
                                        case "data_message_current":
                                            obj = runtime.Data.MessageCurrent?.GetType();
                                            break;
                                        case "data_message_active":
                                            if (runtime.Data.MessageActive != null)
                                                obj = typeof(RestUserMessage);
                                            break;
                                        case "data_emoji_active":
                                            if (runtime.Data.EmojiActive != null)
                                                obj = typeof(IEmote);
                                            break;
                                        case "data_channel_current":
                                            obj = runtime.Data.ChannelCurrent?.GetType();
                                            break;
                                        case "data_channel_active":
                                            obj = runtime.Data.ChannelActive?.GetType();
                                            break;
                                        case "data_category_current":
                                            if (runtime.Data.CategoryCurrent != null)
                                                obj = typeof(RestCategoryChannel);
                                            break;
                                        case "data_category_active":
                                            obj = runtime.Data.CategoryActive?.GetType();
                                            break;
                                        case "data_webhook_active":
                                            if (runtime.Data.WebhookActive != null)
                                                obj = typeof(RestWebhook);
                                            break;
                                        case "data_server_current":
                                            if (runtime.Data.ServerCurrent != null)
                                                obj = typeof(RestGuild);
                                            break;
                                        case "data_server_active":
                                            obj = runtime.Data.ServerActive?.GetType();
                                            break;
                                        case "data_role_active":
                                            obj = runtime.Data.RoleActive?.GetType();
                                            break;
                                        case "data_member_app":
                                            if (runtime.Data.MemberApp != null)
                                                obj = typeof(RestGuildUser);
                                            break;
                                        case "data_member_current":
                                            obj = runtime.Data.MemberCurrent?.GetType();
                                            break;
                                        case "data_member_active":
                                            obj = runtime.Data.MemberActive?.GetType();
                                            break;
                                        case "data_user_app":
                                            if (runtime.Data.UserApp != null)
                                                obj = typeof(RestUser);
                                            break;
                                        case "variables_get":
                                            object? variable = runtime.GetVariableFromBlock(inputBlock);
                                            obj = variable?.GetType();
                                            break;
                                    }

                                    if (obj != null)
                                    {
                                        switch (compare)
                                        {
                                            case "is_message":
                                                return InvertIfValueBool(runtime, obj is RestMessage, valueBlock);
                                            case "is_channel":
                                                return InvertIfValueBool(runtime, obj is RestChannel, valueBlock);
                                            case "is_category":
                                                return InvertIfValueBool(runtime, obj is RestCategoryChannel, valueBlock);
                                            case "is_webhook":
                                                return InvertIfValueBool(runtime, obj is RestWebhook, valueBlock);
                                            case "is_server":
                                                return InvertIfValueBool(runtime, obj is RestGuild, valueBlock);
                                            case "is_emoji":
                                                return InvertIfValueBool(runtime, obj is IEmote, valueBlock);
                                            case "is_role":
                                                return InvertIfValueBool(runtime, obj is RestRole, valueBlock);
                                            case "is_member":
                                                return InvertIfValueBool(runtime, obj is RestGuildUser, valueBlock);
                                            case "is_user":
                                                return InvertIfValueBool(runtime, obj is RestUser, valueBlock);
                                        }
                                    }
                                }
                                return InvertIfValueBool(runtime, false, valueBlock);
                        }
                    }
                    break;
                case "logic_check_discord_channel":
                    {
                        switch (property)
                        {
                            case "channel":
                                {
                                    switch (compare)
                                    {
                                        case "exists":
                                            {
                                                if (inputBlock.type == "data_channel_current")
                                                {
                                                    return InvertIfValueBool(runtime, runtime.Data.ChannelCurrent != null, valueBlock);
                                                }
                                                else if (inputBlock.type == "data_channel_active")
                                                {
                                                    return InvertIfValueBool(runtime, runtime.Data.ChannelActive != null, valueBlock);
                                                }
                                                else if (inputBlock.type == "variables_get")
                                                {
                                                    object? obj = runtime.GetVariableFromBlock(inputBlock);
                                                    return InvertIfValueBool(runtime, obj is RestChannel, valueBlock);

                                                }
                                            }
                                            break;
                                        case "is_in_category":
                                            {
                                                RestChannel? channel = await runtime.GetChannelFromBlock(inputBlock);
                                                if (channel != null)
                                                {
                                                    if (channel is RestTextChannel tc)
                                                        return InvertIfValueBool(runtime, tc.CategoryId.HasValue, valueBlock);
                                                    else if (channel is RestForumChannel fc)
                                                        return InvertIfValueBool(runtime, fc.CategoryId.HasValue, valueBlock);
                                                }
                                            }
                                            break;
                                        case "is_nsfw":
                                            {
                                                RestChannel? channel = await runtime.GetChannelFromBlock(inputBlock);
                                                if (channel != null)
                                                {
                                                    if (channel is RestTextChannel tc)
                                                        return InvertIfValueBool(runtime, tc.IsNsfw, valueBlock);
                                                    else if (channel is RestForumChannel fc)
                                                        return InvertIfValueBool(runtime, fc.IsNsfw, valueBlock);
                                                }
                                            }
                                            break;
                                    }

                                }
                                return InvertIfValueBool(runtime, false, valueBlock);
                            case "type":
                                {
                                    RestChannel? channel = await runtime.GetChannelFromBlock(inputBlock);
                                    if (channel != null)
                                    {
                                        switch (compare)
                                        {
                                            case "is_dm":
                                                return InvertIfValueBool(runtime, channel.GetChannelType() == ChannelType.DM, valueBlock);
                                            case "is_group":
                                                return InvertIfValueBool(runtime, channel.GetChannelType() == ChannelType.Group, valueBlock);
                                            case "is_any_guild_channel":
                                                return InvertIfValueBool(runtime, channel is RestGuildChannel, valueBlock);
                                            case "is_any_textable_channel":
                                                return InvertIfValueBool(runtime, channel is IRestMessageChannel, valueBlock);
                                            case "is_any_guild_textable_channel":
                                                return InvertIfValueBool(runtime, channel is RestTextChannel, valueBlock);
                                            case "is_any_thread_channel":
                                                return InvertIfValueBool(runtime, channel is RestThreadChannel, valueBlock);
                                            case "is_any_forum_channel":
                                                return InvertIfValueBool(runtime, channel is RestForumChannel, valueBlock);
                                            case "is_text_channel":
                                                return InvertIfValueBool(runtime, channel.GetChannelType() == ChannelType.Text, valueBlock);
                                            case "is_voice_channel":
                                                return InvertIfValueBool(runtime, channel.GetChannelType() == ChannelType.Voice, valueBlock);
                                            case "is_news_channel":
                                                return InvertIfValueBool(runtime, channel.GetChannelType() == ChannelType.News, valueBlock);
                                            case "is_forum_channel":
                                                return InvertIfValueBool(runtime, channel.GetChannelType() == ChannelType.Forum, valueBlock);
                                            case "is_media_channel":
                                                return InvertIfValueBool(runtime, channel.GetChannelType() == ChannelType.Media, valueBlock);
                                            case "is_stage_channel":
                                                return InvertIfValueBool(runtime, channel.GetChannelType() == ChannelType.Stage, valueBlock);
                                            case "is_public_thread":
                                                return InvertIfValueBool(runtime, channel.GetChannelType() == ChannelType.PublicThread, valueBlock);
                                            case "is_private_thread":
                                                return InvertIfValueBool(runtime, channel.GetChannelType() == ChannelType.PrivateThread, valueBlock);

                                        }
                                    }

                                }
                                return InvertIfValueBool(runtime, false, valueBlock);
                            case "id":
                                {
                                    RestChannel? channel = await runtime.GetChannelFromBlock(inputBlock);
                                    if (channel != null)
                                        return await CheckString(runtime, channel.Id.ToString(), compare, valueBlock);
                                }
                                break;
                            case "name":
                                {
                                    RestChannel? channels = await runtime.GetChannelFromBlock(inputBlock);
                                    if (channels != null)
                                    {
                                        if (channels is IRestMessageChannel mc)
                                            return await CheckString(runtime, mc.Name, compare, valueBlock);
                                        if (channels is RestForumChannel fc)
                                            return await CheckString(runtime, fc.Name, compare, valueBlock);
                                    }
                                }
                                break;
                            case "category_id":
                                {
                                    RestChannel? channels = await runtime.GetChannelFromBlock(inputBlock);
                                    if (channels != null)
                                    {
                                        if (channels is RestTextChannel mc)
                                            return await CheckString(runtime, mc.CategoryId.GetValueOrDefault(0).ToString(), compare, valueBlock);
                                        if (channels is RestForumChannel fc)
                                            return await CheckString(runtime, fc.CategoryId.GetValueOrDefault(0).ToString(), compare, valueBlock);
                                    }
                                }
                                break;
                            case "guild_id":
                                {
                                    RestChannel? channels = await runtime.GetChannelFromBlock(inputBlock);
                                    if (channels != null)
                                    {
                                        if (channels is RestTextChannel mc)
                                            return await CheckString(runtime, mc.GuildId.ToString(), compare, valueBlock);
                                        if (channels is RestForumChannel fc)
                                            return await CheckString(runtime, fc.GuildId.ToString(), compare, valueBlock);
                                    }
                                }
                                break;
                            case "position":
                                {
                                    if (valueBlock == null)
                                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value block is missing.");

                                    RestChannel? channels = await runtime.GetChannelFromBlock(inputBlock);
                                    if (channels != null)
                                    {
                                        int? Number = await runtime.GetIntFromBlock(valueBlock);
                                        if (Number.HasValue)
                                        {
                                            if (channels is RestTextChannel mc)
                                                return CheckInt(mc.Position, Number.Value, null, compare);
                                            if (channels is RestForumChannel fc)
                                                return CheckInt(fc.Position, Number.Value, null, compare);
                                        }

                                    }
                                }
                                break;
                        }
                    }
                    break;
                case "logic_check_discord_server":
                    {
                        switch (property)
                        {
                            case "server":
                                {
                                    switch (compare)
                                    {
                                        case "exists":
                                            {
                                                if (inputBlock.type == "data_server_current")
                                                {
                                                    return InvertIfValueBool(runtime, runtime.Data.ServerCurrent != null, valueBlock);
                                                }
                                                else if (inputBlock.type == "data_server_active")
                                                {
                                                    return InvertIfValueBool(runtime, runtime.Data.ServerActive != null, valueBlock);
                                                }
                                                else if (inputBlock.type == "variables_get")
                                                {
                                                    object? obj = runtime.GetVariableFromBlock(inputBlock);
                                                    return InvertIfValueBool(runtime, obj is RestGuild, valueBlock);

                                                }
                                            }
                                            return InvertIfValueBool(runtime, false, valueBlock);
                                    }
                                }
                                break;
                            case "id":
                                {
                                    RestGuild? guild = await runtime.GetServerFromBlock(inputBlock);
                                    if (guild != null)
                                        return await CheckString(runtime, guild.Id.ToString(), compare, valueBlock);
                                }
                                break;
                            case "name":
                                {
                                    RestGuild? guild = await runtime.GetServerFromBlock(inputBlock);
                                    if (guild != null)
                                        return await CheckString(runtime, guild.Name, compare, valueBlock);
                                }
                                break;
                            case "owner_id":
                                {
                                    RestGuild? guild = await runtime.GetServerFromBlock(inputBlock);
                                    if (guild != null)
                                        return await CheckString(runtime, guild.OwnerId.ToString(), compare, valueBlock);
                                }
                                break;
                        }
                    }
                    break;
                case "logic_check_discord_message":
                    {
                        switch (property)
                        {
                            case "message":
                                {
                                    switch (compare)
                                    {
                                        case "exists":
                                            {
                                                if (inputBlock.type == "data_message_current")
                                                    return InvertIfValueBool(runtime, runtime.Data.MessageCurrent != null, valueBlock);
                                                else if (inputBlock.type == "data_message_active")
                                                    return InvertIfValueBool(runtime, runtime.Data.MessageActive != null, valueBlock);
                                                else if (inputBlock.type == "variables_get")
                                                {
                                                    object? obj = runtime.GetVariableFromBlock(inputBlock);
                                                    return InvertIfValueBool(runtime, obj is RestMessage, valueBlock);

                                                }
                                            }
                                            return InvertIfValueBool(runtime, false, valueBlock);
                                        case "has_attachments":
                                            {
                                                RestMessage? message = await runtime.GetMessageFromBlock(inputBlock);
                                                if (message != null)
                                                {
                                                    return InvertIfValueBool(runtime, message.Attachments.Count != 0, valueBlock);
                                                }
                                            }
                                            return InvertIfValueBool(runtime, false, valueBlock);
                                    }



                                }
                                break;
                            case "type":
                                {
                                    RestMessage? message = await runtime.GetMessageFromBlock(inputBlock);
                                    if (message != null)
                                    {
                                        switch (compare)
                                        {
                                            case "is_user_message":
                                                return InvertIfValueBool(runtime, message.Source == MessageSource.User, valueBlock);
                                            case "is_bot_message":
                                                return InvertIfValueBool(runtime, message.Source == MessageSource.Bot, valueBlock);
                                            case "is_system_message":
                                                return InvertIfValueBool(runtime, message.Source == MessageSource.System, valueBlock);
                                            case "is_interaction_message":
                                                return InvertIfValueBool(runtime, message.Interaction != null, valueBlock);
                                            case "is_webhook_message":
                                                return InvertIfValueBool(runtime, message.Source == MessageSource.Webhook, valueBlock);
                                        }
                                    }
                                }
                                return InvertIfValueBool(runtime, false, valueBlock);
                            case "id":
                                {
                                    RestMessage? message = await runtime.GetMessageFromBlock(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.Id.ToString(), compare, valueBlock);
                                }
                                break;
                            case "content":
                                {
                                    RestMessage? message = await runtime.GetMessageFromBlock(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.Content, compare, valueBlock);
                                }
                                break;
                        }
                    }
                    break;
                case "logic_check_discord_member":
                    {
                        switch (property)
                        {
                            case "member":
                                {
                                    switch (compare)
                                    {
                                        case "exists":
                                            {
                                                if (inputBlock.type == "data_member_current")
                                                    return InvertIfValueBool(runtime, runtime.Data.MemberCurrent != null, valueBlock);
                                                else if (inputBlock.type == "data_member_active")
                                                    return InvertIfValueBool(runtime, runtime.Data.MemberActive != null, valueBlock);
                                                else if (inputBlock.type == "data_member_app")
                                                    return InvertIfValueBool(runtime, runtime.Data.MemberApp != null, valueBlock);
                                                else if (inputBlock.type == "variables_get")
                                                {
                                                    object? obj = runtime.GetVariableFromBlock(inputBlock);
                                                    return InvertIfValueBool(runtime, obj is RestGuildUser, valueBlock);

                                                }
                                            }
                                            return InvertIfValueBool(runtime, false, valueBlock);
                                        case "has_permission":
                                            {
                                                if (valueBlock == null)
                                                    return false;

                                                RestGuildUser? member = await runtime.GetMemberFromBlock(inputBlock);
                                                if (member == null)
                                                    throw new RuntimeError(RuntimeErrorType.Runtime, "Could not find member to check logic permissions with.");

                                                GuildPermissions? perms = runtime.GetPermissionsFromBlock(valueBlock);
                                                if (perms != null)
                                                {
                                                    return DiscordPermissions.HasPermission(perms.Value, member.InteractionGuildPermissions.HasValue ? member.InteractionGuildPermissions.Value : member.GuildPermissions);
                                                }
                                            }
                                            break;
                                        case "not_has_permission":
                                            {
                                                if (valueBlock == null)
                                                    return true;

                                                RestGuildUser? member = await runtime.GetMemberFromBlock(inputBlock);
                                                if (member == null)
                                                    throw new RuntimeError(RuntimeErrorType.Runtime, "Could not find member to check logic permissions with.");


                                                GuildPermissions? perms = runtime.GetPermissionsFromBlock(valueBlock);
                                                if (perms != null)
                                                {
                                                    return !DiscordPermissions.HasPermission(perms.Value, member.InteractionGuildPermissions.HasValue ? member.InteractionGuildPermissions.Value : member.GuildPermissions);
                                                }
                                            }
                                            return true;
                                    }
                                }
                                break;
                            case "id":
                                {
                                    RestGuildUser? message = await runtime.GetMemberFromBlock(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.Id.ToString(), compare, valueBlock);
                                }
                                break;
                            case "nickname":
                                {
                                    RestGuildUser? message = await runtime.GetMemberFromBlock(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.Nickname, compare, valueBlock);
                                }
                                break;
                        }
                    }
                    break;
                case "logic_check_discord_user":
                    {
                        switch (property)
                        {
                            case "user":
                                {
                                    switch (compare)
                                    {
                                        case "exists":
                                            {
                                                if (inputBlock.type == "data_user_current")
                                                    return InvertIfValueBool(runtime, runtime.Data.UserCurrent != null, valueBlock);
                                                else if (inputBlock.type == "data_user_active")
                                                    return InvertIfValueBool(runtime, runtime.Data.UserActive != null, valueBlock);
                                                else if (inputBlock.type == "data_user_app")
                                                    return InvertIfValueBool(runtime, runtime.Data.UserApp != null, valueBlock);
                                                else if (inputBlock.type == "variables_get")
                                                {
                                                    object? obj = runtime.GetVariableFromBlock(inputBlock);
                                                    return InvertIfValueBool(runtime, obj is RestUser, valueBlock);

                                                }
                                            }
                                            return InvertIfValueBool(runtime, false, valueBlock);
                                        case "is_user":
                                            {
                                                RestUser? message = await runtime.GetUserFromBlock(inputBlock);
                                                if (message != null)
                                                    return InvertIfValueBool(runtime, !message.IsBot, valueBlock);
                                            }
                                            return InvertIfValueBool(runtime, false, valueBlock);
                                        case "is_bot":
                                            {
                                                RestUser? message = await runtime.GetUserFromBlock(inputBlock);
                                                if (message != null)
                                                    return InvertIfValueBool(runtime, message.IsBot, valueBlock);
                                            }
                                            return InvertIfValueBool(runtime, false, valueBlock);
                                    }
                                }
                                break;
                            case "id":
                                {
                                    RestUser? message = await runtime.GetUserFromBlock(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.Id.ToString(), compare, valueBlock);
                                }
                                break;
                            case "username":
                                {
                                    RestUser? message = await runtime.GetUserFromBlock(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.Username, compare, valueBlock);
                                }
                                break;
                            case "display_name":
                                {
                                    RestUser? message = await runtime.GetUserFromBlock(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.GlobalName, compare, valueBlock);
                                }
                                break;
                            case "flags":
                                {
                                    RestUser? message = await runtime.GetUserFromBlock(inputBlock);
                                    if (message != null && message.PublicFlags.HasValue)
                                    {
                                        switch (compare)
                                        {
                                            case "is_active_developer":
                                                return CheckBool(runtime, message.PublicFlags.Value.HasFlag(UserProperties.ActiveDeveloper), valueBlock, compare);
                                            case "is_early_verified_developer":
                                                return CheckBool(runtime, message.PublicFlags.Value.HasFlag(UserProperties.EarlyVerifiedBotDeveloper), valueBlock, compare);
                                        }
                                    }

                                }
                                return InvertIfValueBool(runtime, false, valueBlock);
                        }
                    }
                    break;
                case "logic_check_discord_role":
                    {
                        switch (property)
                        {
                            case "role":
                                {
                                    switch (compare)
                                    {
                                        case "exists":
                                            {
                                                if (inputBlock.type == "data_role_active")
                                                    return InvertIfValueBool(runtime, runtime.Data.RoleActive != null, valueBlock);
                                                else if (inputBlock.type == "variables_get")
                                                {
                                                    object? obj = runtime.GetVariableFromBlock(inputBlock);
                                                    return InvertIfValueBool(runtime, obj is RestRole, valueBlock);

                                                }
                                            }
                                            return InvertIfValueBool(runtime, false, valueBlock);
                                    }
                                }
                                break;
                            case "id":
                                {
                                    RestRole? message = await runtime.GetRoleFromBlock(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.Id.ToString(), compare, valueBlock);
                                }
                                break;
                            case "name":
                                {
                                    RestRole? message = await runtime.GetRoleFromBlock(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.Name, compare, valueBlock);
                                }
                                break;
                        }
                    }
                    break;
                case "logic_check_discord_category":
                    {
                        switch (property)
                        {
                            case "category":
                                {
                                    switch (compare)
                                    {
                                        case "exists":
                                            {
                                                if (inputBlock.type == "data_category_current")
                                                    return InvertIfValueBool(runtime, runtime.Data.CategoryCurrent != null, valueBlock);
                                                else if (inputBlock.type == "data_category_active")
                                                    return InvertIfValueBool(runtime, runtime.Data.CategoryActive != null, valueBlock);
                                                else if (inputBlock.type == "variables_get")
                                                {
                                                    object? obj = runtime.GetVariableFromBlock(inputBlock);
                                                    return InvertIfValueBool(runtime, obj is RestCategoryChannel, valueBlock);

                                                }
                                            }
                                            return InvertIfValueBool(runtime, false, valueBlock);
                                    }
                                }
                                break;
                            case "id":
                                {
                                    RestCategoryChannel? message = await runtime.GetCategoryFromBlock(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.Id.ToString(), compare, valueBlock);
                                }
                                break;
                            case "name":
                                {
                                    RestCategoryChannel? message = await runtime.GetCategoryFromBlock(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.Name, compare, valueBlock);
                                }
                                break;
                        }
                    }
                    break;
                case "logic_check_discord_webhook":
                    {
                        switch (property)
                        {
                            case "webhook":
                                {
                                    switch (compare)
                                    {
                                        case "exists":
                                            {
                                                if (inputBlock.type == "data_webhook_active")
                                                    return InvertIfValueBool(runtime, runtime.Data.WebhookActive != null, valueBlock);
                                                else if (inputBlock.type == "variables_get")
                                                {
                                                    object? obj = runtime.GetVariableFromBlock(inputBlock);
                                                    return InvertIfValueBool(runtime, obj is RestWebhook, valueBlock);

                                                }
                                            }
                                            return InvertIfValueBool(runtime, false, valueBlock);
                                    }
                                }
                                break;
                            case "id":
                                {
                                    Tuple<RestWebhook?, string>? message = await runtime.GetWebhookFromBlock(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.Item1.Id.ToString(), compare, valueBlock);
                                }
                                break;
                        }
                    }
                    break;
                case "logic_check_discord_emoji":
                    {
                        switch (property)
                        {
                            case "emoji":
                                {
                                    switch (compare)
                                    {
                                        case "exists":
                                            {
                                                if (inputBlock.type == "data_emoji_active")
                                                    return InvertIfValueBool(runtime, runtime.Data.EmojiActive != null, valueBlock);
                                                else if (inputBlock.type == "variables_get")
                                                {
                                                    object? obj = runtime.GetVariableFromBlock(inputBlock);
                                                    return InvertIfValueBool(runtime, obj is IEmote, valueBlock);

                                                }
                                            }
                                            return InvertIfValueBool(runtime, false, valueBlock);
                                    }
                                }
                                break;
                            case "id":
                                {
                                    Tuple<Tuple<ulong, RestGuild?>?, IEmote>? message = await runtime.GetEmojiFromBlockAsync(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, (message.Item2 as GuildEmote)?.Id.ToString(), compare, valueBlock);
                                }
                                break;
                            case "name":
                                {
                                    Tuple<Tuple<ulong, RestGuild?>?, IEmote>? message = await runtime.GetEmojiFromBlockAsync(inputBlock);
                                    if (message != null)
                                        return await CheckString(runtime, message.Item2.Name, compare, valueBlock);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new RuntimeError(RuntimeErrorType.Server, "Failed to check logic condition, invalid check block.");
                    break;
            }
        }
        return false;
    }

    public static bool InvertIfValueBool(DiscordRuntime runtime, bool current, WorkspaceBlock? valueInput)
    {
        bool? hasBlock = null;
        if (valueInput != null)
            runtime.GetBoolFromBlock(valueInput);

        if (hasBlock.HasValue && !hasBlock.Value)
            return !current;

        return current;
    }

    public static async Task<bool> CheckString(DiscordRuntime runtime, string input, string compare, WorkspaceBlock? valueBlock)
    {
        switch (compare)
        {
            case "exists":
                return InvertIfValueBool(runtime, !string.IsNullOrEmpty(input), valueBlock);
            case "equals":
            case "not_equals":
                if (valueBlock == null)
                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value is missing for string check.");

                return input.Equals(await runtime.GetStringFromBlock(valueBlock), StringComparison.OrdinalIgnoreCase);
            case "contains":
            case "not_contains":
                if (valueBlock == null)
                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value is missing for string check.");

                return input.Contains(await runtime.GetStringFromBlock(valueBlock), StringComparison.OrdinalIgnoreCase);
            case "starts_with":
            case "not_starts_with":
                if (valueBlock == null)
                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value is missing for string check.");

                return input.StartsWith(await runtime.GetStringFromBlock(valueBlock), StringComparison.OrdinalIgnoreCase);
            case "ends_with":
            case "not_ends_with":
                if (valueBlock == null)
                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value is missing for string check.");

                return input.EndsWith(await runtime.GetStringFromBlock(valueBlock), StringComparison.OrdinalIgnoreCase);

            case "equals_case":
            case "not_equals_case":
                if (valueBlock == null)
                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value is missing for string check.");

                return input.Equals(await runtime.GetStringFromBlock(valueBlock), StringComparison.Ordinal);

            case "contains_case":
            case "not_contains_case":
                if (valueBlock == null)
                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value is missing for string check.");

                return input.Contains(await runtime.GetStringFromBlock(valueBlock), StringComparison.Ordinal);

            case "starts_with_case":
            case "not_starts_with_case":
                if (valueBlock == null)
                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value is missing for string check.");

                return input.StartsWith(await runtime.GetStringFromBlock(valueBlock), StringComparison.Ordinal);

            case "ends_with_case":
            case "not_ends_with_case":
                if (valueBlock == null)
                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value is missing for string check.");

                return input.EndsWith(await runtime.GetStringFromBlock(valueBlock), StringComparison.Ordinal);

            case "length_equals":
            case "not_length_equals":
                {
                    if (valueBlock == null)
                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value is missing for string check.");

                    int? number = await runtime.GetIntFromBlock(valueBlock);
                    if (number.HasValue)
                        return number.Value == input.Length;

                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, invalid value number.");
                }
            case "length_more_than":
                {
                    if (valueBlock == null)
                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value is missing for string check.");

                    int? number = await runtime.GetIntFromBlock(valueBlock);
                    if (number.HasValue)
                        return input.Length > number.Value;

                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, invalid value number.");
                }
            case "length_less_than":
                {
                    if (valueBlock == null)
                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value is missing for string check.");

                    int? number = await runtime.GetIntFromBlock(valueBlock);
                    if (number.HasValue)
                        return input.Length < number.Value;

                    throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, invalid value number.");
                }
            case "length_between":
                {
                    if (valueBlock == null)
                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value is missing for string check.");

                    int? number = await runtime.GetIntFromBlock(valueBlock);
                    if (number.HasValue)
                    {
                        int[] between = new int[]
                        {
                            (int)valueBlock.fields["first"],
                            (int)valueBlock.fields["second"]
                        };

                        if (between[1] < between[0])
                            throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, invalid between number.");

                        if (input.Length >= between[0] && input.Length <= between[1])
                            return true;
                    }
                }
                break;

        }
        return false;
    }

    public static async Task<bool> CheckNumber(DiscordRuntime runtime, WorkspaceBlock inputBlock, WorkspaceBlock? valueBlock, string compare)
    {
        int? intNum = await runtime.GetIntFromBlock(inputBlock);
        double? dobNum = await runtime.GetDoubleFromBlock(inputBlock);
        return await CheckNumber(runtime, intNum, dobNum, valueBlock, compare);
    }

    public static bool CheckBool(DiscordRuntime runtime, bool? inputBool, WorkspaceBlock? valueBlock, string compare)
    {
        switch (compare)
        {
            case "exists":
                {
                    if (inputBool != null)
                        return InvertIfValueBool(runtime, true, valueBlock);

                    return InvertIfValueBool(runtime, false, valueBlock);
                }
                break;
            case "equals":
                {
                    if (inputBool != null)
                        return inputBool.Value;
                }
                break;
        }

        return false;
    }

    public static async Task<bool> CheckNumber(DiscordRuntime runtime, int? inputNum, double? inputDob, WorkspaceBlock? valueBlock, string compare)
    {
        if (compare == "exists")
        {
            return InvertIfValueBool(runtime, (inputNum.HasValue || inputDob.HasValue), valueBlock);
        }
        else
        {
            if (valueBlock == null)
                throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value block is missing.");

            int? integer = await runtime.GetIntFromBlock(valueBlock);
            double? doublen = await runtime.GetDoubleFromBlock(valueBlock);

            if (integer.HasValue || doublen.HasValue)
                throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value block is invalid.");

            if (integer.HasValue)
            {
                int[] between = null;
                if (compare == "between")
                {
                    if (valueBlock.type == "math_number_between")
                    {
                        between = new int[]
                        {
                            (int)valueBlock.fields["first"],
                            (int)valueBlock.fields["second"]
                        };
                    }
                    else
                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value block is not between type.");
                }

                if (inputNum.HasValue)
                    return CheckInt(inputNum.Value, integer.Value, between, compare);
            }

            if (doublen.HasValue)
            {
                double[] between = null;
                if (compare == "between")
                {
                    if (valueBlock.type == "math_number_between")
                    {
                        between = new double[]
                        {
                            (double)valueBlock.fields["first"],
                            (double)valueBlock.fields["second"]
                        };
                    }
                    else
                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, value block is not between type.");
                }

                if (inputDob.HasValue)
                    return CheckDouble(inputDob.Value, doublen.Value, between, compare);
            }

        }

        return false;
    }

    public static bool CheckInt(int input, int value, int[]? between, string compare)
    {
        switch (compare)
        {
            case "equals":
                return input == value;
            case "not_equals":
                return input != value;
            case "more_than":
                return input > value;
            case "less_than":
                return input < value;
            case "between":
                {
                    if (between != null)
                    {
                        if (between[1] < between[0])
                            throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, invalid between number.");

                        if (input >= between[0] && input <= between[1])
                            return true;
                    }
                }
                break;
        }
        return false;
    }

    public static bool CheckDouble(double input, double value, double[] between, string compare)
    {
        switch (compare)
        {
            case "equals":
                return input == value;
            case "not_equals":
                return input != value;
            case "more_than":
                return input > value;
            case "less_than":
                return input < value;
            case "between":
                {
                    if (between[1] < between[0])
                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to check logic condition, invalid between number.");

                    if (input >= between[0] && input <= between[1])
                        return true;
                }
                break;
        }
        return false;
    }
}
