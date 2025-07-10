using DevSpaceWeb.Apps.Data;
using DevSpaceWeb.Apps.Runtime.Main;
using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp;

public class DiscordInputs
{
    public static async Task ParseNextBlock(DiscordRuntime runtime, WorkspaceBlock block, string modalDataType = "")
    {
        if (block.enabled)
        {
            string Name = string.Empty;
            if (block.inputs.TryGetValue("name", out WorkspaceBlockConnection? nameBlock) && nameBlock.block != null)
                Name = await runtime.GetStringFromBlock(nameBlock.block);


            if (!string.IsNullOrEmpty(Name))
            {
                RestSlashCommandDataOption? ValueData = (runtime.Interaction as RestSlashCommand).Data.Options.FirstOrDefault(x => x.Name == Name);
                if (ValueData != null)
                {
                    if (block.inputs.TryGetValue("output_variable", out WorkspaceBlockConnection? outputBlock) && outputBlock.block != null)
                    {
                        string Key = outputBlock.block.GetVariableId();
                        switch (ValueData.Type)
                        {
                            case Discord.ApplicationCommandOptionType.Boolean:
                                runtime.SetVariable(Key, (bool)ValueData.Value);
                                break;
                            case Discord.ApplicationCommandOptionType.Integer:
                                runtime.SetVariable(Key, (int)ValueData.Value);
                                break;
                            case Discord.ApplicationCommandOptionType.Number:
                                runtime.SetVariable(Key, (double)ValueData.Value);
                                break;
                            case Discord.ApplicationCommandOptionType.String:
                                runtime.SetVariable(Key, ValueData.Value.ToString());
                                break;
                        }

                    }
                    else if (block.inputs.TryGetValue("output_user", out outputBlock) && outputBlock.block != null)
                    {
                        RestUser User = (RestUser)ValueData.Value;
                        if (User != null)
                        {
                            if (!string.IsNullOrEmpty(modalDataType) && outputBlock.block.type == modalDataType)
                                runtime.ModalDataType = "ur" + User.Id;

                            runtime.Cache.AddUser(User);
                            runtime.SetUserData(outputBlock.block, User);
                        }
                    }
                    else if (block.inputs.TryGetValue("output_member", out outputBlock) && outputBlock.block != null)
                    {
                        RestGuildUser Member = (RestGuildUser)ValueData.Value;
                        if (Member != null)
                        {
                            if (!string.IsNullOrEmpty(modalDataType) && outputBlock.block.type == modalDataType)
                            {
                                runtime.ModalDataType = $"mb{Member.GuildId}-{Member.Id}";
                            }


                            runtime.Cache.AddMember(Member);
                            runtime.SetMemberData(outputBlock.block, Member);
                        }
                    }
                    else if (block.inputs.TryGetValue("output_channel", out outputBlock) && outputBlock.block != null)
                    {
                        RestChannel Chan = (RestChannel)ValueData.Value;
                        if (Chan != null)
                        {
                            if (!string.IsNullOrEmpty(modalDataType) && outputBlock.block.type == modalDataType)
                            {
                                if (Chan.GetChannelType() == ChannelType.Category)
                                    runtime.ModalDataType = "ct" + Chan.Id;
                                else
                                    runtime.ModalDataType = "ch" + Chan.Id;
                            }

                            runtime.Cache.AddChannel(Chan);
                            runtime.SetChannelData(outputBlock.block, Chan);
                        }
                    }
                    else if (block.inputs.TryGetValue("output_role", out outputBlock) && outputBlock.block != null)
                    {
                        RestRole Role = (RestRole)ValueData.Value;
                        if (Role != null)
                        {
                            if (!string.IsNullOrEmpty(modalDataType) && outputBlock.block.type == modalDataType)
                                runtime.ModalDataType = $"rl{Role.GuildId}-{Role.Id}";

                            runtime.Cache.AddRole(Role);
                            runtime.SetRoleData(outputBlock.block, Role.GuildId, Role);
                        }
                    }
                    else if (block.inputs.TryGetValue("output_mentionable", out outputBlock) && outputBlock.block != null)
                    {
                        if (ValueData.Value is RestGuildUser gu)
                        {
                            if (!string.IsNullOrEmpty(modalDataType) && outputBlock.block.type == modalDataType)
                                runtime.ModalDataType = $"mb{gu.GuildId}-{gu.Id}";

                            runtime.Cache.AddMember(gu);
                            runtime.SetMemberData(outputBlock.block, gu);
                        }
                        else if (ValueData.Value is RestUser u)
                        {
                            if (!string.IsNullOrEmpty(modalDataType) && outputBlock.block.type == modalDataType)
                                runtime.ModalDataType = "ur" + u.Id;

                            runtime.Cache.AddUser(u);
                            runtime.SetUserData(outputBlock.block, u);
                        }
                        else if (ValueData.Value is RestRole r)
                        {
                            if (!string.IsNullOrEmpty(modalDataType) && outputBlock.block.type == modalDataType)
                                runtime.ModalDataType = $"rl{r.GuildId}-{r.Id}";

                            runtime.Cache.AddRole(r);
                            runtime.SetRoleData(outputBlock.block, r.Id, r);
                        }
                        else if (ValueData.Value is RestChannel c)
                        {
                            if (!string.IsNullOrEmpty(modalDataType) && outputBlock.block.type == modalDataType)
                            {
                                if (c.GetChannelType() == ChannelType.Category)
                                    runtime.ModalDataType = "ct" + c.Id;
                                else
                                    runtime.ModalDataType = "ch" + c.Id;
                            }

                            runtime.Cache.AddChannel(c);
                            runtime.SetChannelData(outputBlock.block, c);
                        }
                    }
                    else if (block.inputs.TryGetValue("output_file", out outputBlock) && outputBlock.block != null)
                    {
                        Attachment Attach = (Attachment)ValueData.Value;
                        runtime.SetFileData(outputBlock.block, Attach);
                    }
                }
            }
        }

        if (block.next != null && block.next.block != null)
            await ParseNextBlock(runtime, block.next.block, modalDataType);
    }



    public static async Task<List<DiscordAppCommandInput>> Parse(WorkspaceBlock block)
    {
        InputCache cache = new InputCache();
        await NextInput(new MainRuntime(), cache, block);

        return cache.inputs;
    }

    public class InputCache
    {
        public List<DiscordAppCommandInput> inputs = new List<DiscordAppCommandInput>();
    }

    private static async Task NextInput(MainRuntime runtime, InputCache inputs, WorkspaceBlock block)
    {
        if (block.enabled && block.type.StartsWith("input_"))
        {
            DiscordAppCommandInputType? Type = GetInputType(block);
            if (Type.HasValue)
            {
                DiscordAppCommandInput Input = new DiscordAppCommandInput
                {
                    Type = Type.Value
                };
                if (block.inputs.TryGetValue("name", out WorkspaceBlockConnection? inputBlock) && inputBlock.block != null)
                    Input.Name = await runtime.GetStringFromBlock(inputBlock.block);

                if (block.inputs.TryGetValue("is_required", out inputBlock) && inputBlock.block != null)
                    Input.IsRequired = (await runtime.GetBoolFromBlock(inputBlock.block)).GetValueOrDefault(false);

                if (block.inputs.TryGetValue("description", out inputBlock) && inputBlock.block != null)
                    Input.Description = await runtime.GetStringFromBlock(inputBlock.block);

                if (block.inputs.TryGetValue("arg_minmax", out inputBlock) && inputBlock.block != null)
                {
                    if (inputBlock.block.fields.TryGetValue("min", out Newtonsoft.Json.Linq.JToken? field))
                        Input.Min = (int)field;

                    if (inputBlock.block.fields.TryGetValue("nax", out field))
                        Input.Max = (int)field;
                }

                if (block.inputs.TryGetValue("obj_input_choices_list", out inputBlock) && inputBlock.block != null)
                {
                    Dictionary<string, string> list = new Dictionary<string, string>();

                    foreach (WorkspaceBlockConnection i in inputBlock.block.inputs.Values)
                    {
                        if (i.block != null && i.block.enabled)
                            list.Add(i.block.fields["name"].ToString(), i.block.fields["value"].ToString());
                    }

                    if (list.Count != 0)
                        Input.Choices = list;
                }

                inputs.inputs.Add(Input);
            }
        }

        if (block.next != null && block.next.block != null)
            await NextInput(runtime, inputs, block.next.block);
    }

    private static DiscordAppCommandInputType? GetInputType(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "input_string":
                return DiscordAppCommandInputType.String;
            case "input_integer":
                return DiscordAppCommandInputType.Integer;
            case "input_boolean":
                return DiscordAppCommandInputType.Boolean;
            case "input_user":
                return DiscordAppCommandInputType.User;
            case "input_member":
                return DiscordAppCommandInputType.Member;
            case "input_channel":
                return DiscordAppCommandInputType.Channel;
            case "input_role":
                return DiscordAppCommandInputType.Role;
            case "input_mentionable":
                return DiscordAppCommandInputType.Mentionable;
            case "input_number":
                return DiscordAppCommandInputType.Number;
            case "input_attachment":
                return DiscordAppCommandInputType.Attachment;
        }

        return null;
    }
}
