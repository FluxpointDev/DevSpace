using DevSpaceWeb.Apps.Runtime.DiscordApp;
using Discord;

namespace DevSpaceWeb.Apps.Runtime.Main.Blocks;

public class SetVariableBlock : IActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        string Key = Block.GetVariableId();
        if (string.IsNullOrEmpty(Key))
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, variable key is invalid.");

        RequestBlocks_Block? input = null;
        if (Block.inputs.TryGetValue("VALUE", out RequestBlocksBlock? inputBlock) && inputBlock.block != null)
            input = inputBlock.block;

        if (input == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, input is missing.");

        if (input.type.Contains("_"))
        {
            string[] Split = input.type.Split('_');
            if (Split.Length == 3)
            {
                switch ($"{Split[0]}_{Split[1]}")
                {
                    case "data_string":
                        {
                            Runtime.SetVariable(Key, await Runtime.GetStringFromBlock(input));
                            return null;
                        }
                        break;
                    case "data_permission":
                        {
                            GuildPermissions? perm = (Runtime as DiscordRuntime).GetPermissionsFromBlock(input);
                            if (perm == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not parse permissions.");
                            Runtime.SetVariable(Key, perm.Value);
                            return null;
                        }
                        break;
                    case "data_message":
                        {
                            Discord.Rest.RestMessage? message = await (Runtime as DiscordRuntime).GetMessageFromBlock(input);
                            if (message == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not get message.");

                            Runtime.SetVariable(Key, message);
                            return null;
                        }
                        break;
                    case "data_file":
                        {
                            FileData? message = Runtime.GetFileFromBlock(input);
                            if (message == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not get file.");

                            Runtime.SetVariable(Key, message);
                            return null;
                        }
                        break;
                    case "data_emoji":
                        {
                            Tuple<Tuple<ulong, Discord.Rest.RestGuild?>?, IEmote>? message = await (Runtime as DiscordRuntime).GetEmojiFromBlockAsync(input);
                            if (message == null || message.Item2 == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not get emoji.");

                            Runtime.SetVariable(Key, message.Item2);
                            return null;
                        }
                        break;
                    case "data_channel":
                        {
                            Discord.Rest.RestChannel? message = await (Runtime as DiscordRuntime).GetChannelFromBlock(input);
                            if (message == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not get channel.");

                            Runtime.SetVariable(Key, message);
                            return null;
                        }
                        break;
                    case "data_category":
                        {
                            Discord.Rest.RestCategoryChannel? message = await (Runtime as DiscordRuntime).GetCategoryFromBlock(input);
                            if (message == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not get category.");

                            Runtime.SetVariable(Key, message);
                            return null;
                        }
                        break;
                    case "data_webhook":
                        {
                            Tuple<Discord.Rest.RestWebhook?, string>? message = await (Runtime as DiscordRuntime).GetWebhookFromBlock(input);
                            if (message == null || message.Item1 == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not get webhook.");

                            Runtime.SetVariable(Key, message.Item1);
                            return null;
                        }
                        break;
                    case "data_server":
                        {
                            Discord.Rest.RestGuild? message = await (Runtime as DiscordRuntime).GetServerFromBlock(input);
                            if (message == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not get server.");

                            Runtime.SetVariable(Key, message);
                            return null;
                        }
                        break;
                    case "data_role":
                        {
                            Discord.Rest.RestRole? message = await (Runtime as DiscordRuntime).GetRoleFromBlock(input);
                            if (message == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not get role.");

                            Runtime.SetVariable(Key, message);
                            return null;
                        }
                        break;
                    case "data_member":
                        {
                            Discord.Rest.RestGuildUser? message = await (Runtime as DiscordRuntime).GetMemberFromBlock(input);
                            if (message == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not get member.");

                            Runtime.SetVariable(Key, message);
                            return null;
                        }
                        break;
                    case "data_user":
                        {
                            Discord.Rest.RestUser? message = await (Runtime as DiscordRuntime).GetUserFromBlock(input);
                            if (message == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not get user.");

                            Runtime.SetVariable(Key, message);
                            return null;
                        }
                        break;
                    case "data_json":
                        {
                            Newtonsoft.Json.Linq.JObject? message = Runtime.GetJsonFromBlock(input);
                            if (message == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not get json.");

                            Runtime.SetVariable(Key, message);
                            return null;
                        }
                        break;
                    case "data_response":
                        {
                            ResponseData? message = Runtime.GetResponseFromBlock(input);
                            if (message == null)
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set variable, could not get response.");

                            Runtime.SetVariable(Key, message);
                            return null;
                        }
                        break;
                }
            }
        }
        switch (input.type)
        {
            case "text":
            case "text_multiline":
                {
                    Runtime.SetVariable(Key, await Runtime.GetStringFromBlock(input));
                }
                break;
            case "math_number":
                {
                    double? dobNum = await Runtime.GetDoubleFromBlock(input);
                    if (dobNum.HasValue)
                    {
                        Runtime.SetVariable(Key, dobNum.Value);
                        return null;
                    }

                    int? intNum = await Runtime.GetIntFromBlock(input);
                    if (intNum.HasValue)
                        Runtime.SetVariable(Key, intNum.Value);
                }
                break;
            case "logic_boolean":
                {
                    bool? boolv = await Runtime.GetBoolFromBlock(input);
                    if (boolv.HasValue)
                        Runtime.SetVariable(Key, boolv.Value);
                }
                break;
            case "variables_get":
                {
                    object? Obj = Runtime.GetVariableFromBlock(input);
                    if (Obj != null)
                        Runtime.SetVariable(Key, Obj);
                }
                break;
        }

        return null;
    }
}
