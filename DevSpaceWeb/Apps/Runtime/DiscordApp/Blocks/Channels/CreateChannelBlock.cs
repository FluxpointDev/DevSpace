using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Channels;

public class CreateChannelBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestGuild? Guild = null;
        if (Block.inputs.TryGetValue("server", out RequestBlocksBlock? servBlock) && servBlock.block != null)
            Guild = await Runtime.GetServerFromBlock(servBlock.block);

        if (Guild == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create channel, could not find server.");

        if (Guild.Id == Runtime.Interaction.GuildId.GetValueOrDefault() && !Runtime.Interaction.Permissions!.Value.ViewChannel)
            return Runtime.GetAppPermissionError(GuildPermission.ViewChannel);

        RequestBlocks_Block? ChanBlock = null;
        if (Block.inputs.TryGetValue("output_channel", out RequestBlocksBlock? hookBlock) && hookBlock.block != null)
            ChanBlock = hookBlock.block;

        if (Guild.Id == Runtime.Interaction.GuildId.GetValueOrDefault() && !Runtime.Interaction.Permissions!.Value.ManageChannels)
            return Runtime.GetAppPermissionError(GuildPermission.ManageChannels);

        switch (Block.type)
        {
            case "action_create_text_channel":
                {
                    ChannelObjectBlock? chan = null;
                    if (Block.inputs.TryGetValue("obj_text_channel", out RequestBlocksBlock? catBlock) && catBlock.block != null)
                        chan = DiscordBlocks.Parse(Runtime, catBlock.block) as ChannelObjectBlock;

                    if (chan == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create text channel, text channel object data is missing.");

                    int? Pos = await chan.Position();
                    if (Pos.HasValue && Pos.Value < 0)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create text channel, position can't be less than 0.");

                    RestCategoryChannel? cat = await chan.Category();

                    string topic = await chan.Topic();

                    int? Slowmode = await chan.Slowmode();
                    if (Slowmode.HasValue)
                    {
                        switch (Slowmode.Value)
                        {
                            case 60:
                            case 1440:
                            case 4320:
                            case 10080:
                                break;
                            default:
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create text channel, slowmode needs to be 60, 1440, 4320 or 10080.");
                        }
                    }

                    GuildPermissions? Perms = chan.Permissions();
                    string ChanName = await chan.Name();
                    bool ChanIsNsfw = await chan.IsNsfw();

                    RestTextChannel Channel = await Guild.CreateTextChannelAsync(ChanName, x =>
                    {
                        x.Position = !Pos.HasValue ? Optional<int>.Unspecified : Optional.Create(Pos.Value);
                        x.SlowModeInterval = !Slowmode.HasValue ? Optional<int>.Unspecified : Optional.Create(Slowmode.Value);
                        x.IsNsfw = ChanIsNsfw;
                        x.CategoryId = cat != null ? cat.Id : Optional<ulong?>.Unspecified;
                        x.Topic = !string.IsNullOrEmpty(topic) ? topic : Optional<string>.Unspecified;
                    });

                    if (Channel == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create text channel, could not find channel.");

                    if (ChanBlock != null)
                    {
                        Runtime.Cache.AddChannel(Channel);
                        Runtime.SetChannelData(ChanBlock, Channel);
                    }

                }
                break;
            case "action_create_voice_channel":
                {
                    ChannelObjectBlock? chan = null;
                    if (Block.inputs.TryGetValue("obj_voice_channel", out RequestBlocksBlock? catBlock) && catBlock.block != null)
                        chan = DiscordBlocks.Parse(Runtime, catBlock.block) as ChannelObjectBlock;

                    if (chan == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create voice channel, voice channel object data is missing.");

                    int? Pos = await chan.Position();
                    if (Pos.HasValue && Pos.Value < 0)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create voice channel, position can't be less than 0.");

                    RestCategoryChannel? cat = await chan.Category();

                    int? Slowmode = await chan.Slowmode();
                    if (Slowmode.HasValue)
                    {
                        switch (Slowmode.Value)
                        {
                            case 60:
                            case 1440:
                            case 4320:
                            case 10080:
                                break;
                            default:
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create voice channel, slowmode needs to be 60, 1440, 4320 or 10080.");
                        }
                    }

                    GuildPermissions? Perms = chan.Permissions();
                    string ChanName = await chan.Name();
                    bool ChanIsNsfw = await chan.IsNsfw();
                    RestVoiceChannel Channel = await Guild.CreateVoiceChannelAsync(ChanName, x =>
                    {
                        x.Position = Pos == null || !Pos.HasValue ? Optional<int>.Unspecified : Optional.Create(Pos.Value);
                        x.SlowModeInterval = Slowmode == null || !Slowmode.HasValue ? Optional<int>.Unspecified : Optional.Create(Slowmode.Value);
                        x.IsNsfw = ChanIsNsfw;
                        x.CategoryId = cat != null ? cat.Id : Optional<ulong?>.Unspecified;
                    });

                    if (Channel == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create voice channel, could not find channel.");

                    if (ChanBlock != null)
                    {
                        Runtime.Cache.AddChannel(Channel);
                        Runtime.SetChannelData(ChanBlock, Channel);
                    }

                }
                break;
            case "action_create_forum_channel":
                {
                    ChannelObjectBlock? chan = null;
                    if (Block.inputs.TryGetValue("obj_forum_channel", out RequestBlocksBlock? catBlock) && catBlock.block != null)
                        chan = DiscordBlocks.Parse(Runtime, catBlock.block) as ChannelObjectBlock;

                    if (chan == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create forum channel, forum channel object data is missing.");

                    int? Pos = await chan.Position();
                    if (Pos.HasValue && Pos.Value < 0)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create forum channel, position can't be less than 0.");

                    RestCategoryChannel? cat = await chan.Category();

                    int? Slowmode = await chan.Slowmode();
                    if (Slowmode.HasValue)
                    {
                        switch (Slowmode.Value)
                        {
                            case 60:
                            case 1440:
                            case 4320:
                            case 10080:
                                break;
                            default:
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create forum channel, slowmode needs to be 60, 1440, 4320 or 10080.");
                        }
                    }

                    GuildPermissions? Perms = chan.Permissions();
                    string ChanName = await chan.Name();
                    bool ChanIsNsfw = await chan.IsNsfw();
                    RestForumChannel Channel = await Guild.CreateForumChannelAsync(ChanName, x =>
                    {
                        x.Position = Pos == null || !Pos.HasValue ? Optional<int>.Unspecified : Optional.Create(Pos.Value);
                        x.DefaultSlowModeInterval = !Slowmode.HasValue ? Optional<int>.Unspecified : Optional.Create(Slowmode.Value);
                        x.IsNsfw = ChanIsNsfw ? Optional.Create<bool>(ChanIsNsfw) : Optional<bool>.Unspecified;
                        x.CategoryId = cat != null ? Optional.Create<ulong?>(cat.Id) : Optional<ulong?>.Unspecified;

                        //x.ThreadCreationInterval
                    });

                    if (Channel == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create forum channel, could not find channel.");

                    if (ChanBlock != null)
                    {
                        Runtime.Cache.AddChannel(Channel);
                        Runtime.SetChannelData(ChanBlock, Channel);
                    }

                }
                break;
            case "action_create_announcement_channel":
                {
                    ChannelObjectBlock? chan = null;
                    if (Block.inputs.TryGetValue("obj_text_channel", out RequestBlocksBlock? catBlock) && catBlock.block != null)
                        chan = DiscordBlocks.Parse(Runtime, catBlock.block) as ChannelObjectBlock;

                    if (chan == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create announcement channel, text channel object data is missing.");

                    int? Pos = await chan.Position();
                    if (Pos.HasValue && Pos.Value < 0)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create announcement channel, position can't be less than 0.");

                    RestCategoryChannel? cat = await chan.Category();

                    string topic = await chan.Topic();

                    int? Slowmode = await chan.Slowmode();
                    if (Slowmode.HasValue)
                    {
                        switch (Slowmode.Value)
                        {
                            case 60:
                            case 1440:
                            case 4320:
                            case 10080:
                                break;
                            default:
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create announcement channel, slowmode needs to be 60, 1440, 4320 or 10080.");
                        }
                    }

                    GuildPermissions? Perms = chan.Permissions();
                    string ChanName = await chan.Name();
                    bool ChanIsNsfw = await chan.IsNsfw();
                    RestTextChannel Channel = await Guild.CreateNewsChannelAsync(ChanName, x =>
                    {
                        x.Position = Pos == null || !Pos.HasValue ? Optional<int>.Unspecified : Optional.Create(Pos.Value);
                        x.SlowModeInterval = Slowmode == null || !Slowmode.HasValue ? Optional<int>.Unspecified : Optional.Create(Slowmode.Value);
                        x.IsNsfw = ChanIsNsfw;
                        x.CategoryId = cat != null ? cat.Id : Optional<ulong?>.Unspecified;
                        x.Topic = !string.IsNullOrEmpty(topic) ? topic : Optional<string>.Unspecified;
                    });

                    if (Channel == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create announcement channel, could not find channel.");

                    if (ChanBlock != null)
                    {
                        Runtime.Cache.AddChannel(Channel);
                        Runtime.SetChannelData(ChanBlock, Channel);
                    }

                }
                break;
            case "action_create_stage_channel":
                {
                    ChannelObjectBlock? chan = null;
                    if (Block.inputs.TryGetValue("obj_stage_channel", out RequestBlocksBlock? catBlock) && catBlock.block != null)
                        chan = DiscordBlocks.Parse(Runtime, catBlock.block) as ChannelObjectBlock;

                    if (chan == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create stage channel, stage channel object data is missing.");

                    int? Pos = await chan.Position();
                    if (Pos.HasValue && Pos.Value < 0)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create stage channel, position can't be less than 0.");

                    RestCategoryChannel? cat = await chan.Category();

                    int? Slowmode = await chan.Slowmode();
                    if (Slowmode.HasValue)
                    {
                        switch (Slowmode.Value)
                        {
                            case 60:
                            case 1440:
                            case 4320:
                            case 10080:
                                break;
                            default:
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create stage channel, slowmode needs to be 60, 1440, 4320 or 10080.");
                        }
                    }

                    GuildPermissions? Perms = chan.Permissions();
                    string ChanName = await chan.Name();
                    bool ChanIsNsfw = await chan.IsNsfw();
                    RestStageChannel Channel = await Guild.CreateStageChannelAsync(ChanName, x =>
                    {
                        x.Position = Pos == null || !Pos.HasValue ? Optional<int>.Unspecified : Optional.Create(Pos.Value);
                        x.SlowModeInterval = Slowmode == null || !Slowmode.HasValue ? Optional<int>.Unspecified : Optional.Create(Slowmode.Value);
                        x.IsNsfw = ChanIsNsfw;
                        x.CategoryId = cat != null ? cat.Id : Optional<ulong?>.Unspecified;
                    });

                    if (Channel == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to create stage channel, could not find channel.");

                    if (ChanBlock != null)
                    {
                        Runtime.Cache.AddChannel(Channel);
                        Runtime.SetChannelData(ChanBlock, Channel);
                    }

                }
                break;
        }

        return null;
    }
}
