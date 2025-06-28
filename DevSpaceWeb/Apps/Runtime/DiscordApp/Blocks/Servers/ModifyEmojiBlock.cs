using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Servers;

public class ModifyEmojiBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        Tuple<Tuple<ulong, RestGuild?>?, IEmote>? Emoji = null;

        if (Block.inputs.TryGetValue("emoji", out RequestBlocksBlock? chanBlock) && chanBlock.block != null)
            Emoji = await Runtime.GetEmojiFromBlockAsync(chanBlock.block);

        if (Emoji == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify emoji, could not find emoji.");

        if (Emoji.Item2 is not GuildEmote)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify emoji, you can't use unicode emojis.");

        if (Emoji.Item1 == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify emoji, could not find server.");

        if (Emoji.Item1.Item1 == Runtime.Interaction.GuildId.GetValueOrDefault() && !Runtime.Interaction.Permissions!.Value.ManageEmojisAndStickers)
            return Runtime.GetAppPermissionError(GuildPermission.ManageEmojisAndStickers);

        EmojiObjectBlock? EmojiObject = null;
        if (Block.inputs.TryGetValue("obj_emoji", out RequestBlocksBlock? webBlock) && webBlock.block != null)
            EmojiObject = DiscordBlocks.Parse(Runtime, webBlock.block) as EmojiObjectBlock;

        if (EmojiObject == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify emoji, emoji object data is missing.");



        List<RestRole>? roles = await EmojiObject.Roles();

        string? EmojiName = await EmojiObject.Name();

        if (Emoji.Item1 == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify emoji, this is not a server emoji.");

        RestGuild Guild = Emoji.Item1.Item2;
        if (Guild == null)
            Guild = await Runtime.GetGuildAsync(Emoji.Item1.Item1);

        if (Guild == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify emoji, could not find Discord server.");

        await Guild.ModifyEmoteAsync(Emoji.Item2 as GuildEmote, x =>
        {
            if (!string.IsNullOrEmpty(EmojiName))
                x.Name = EmojiName;

            if (roles != null)
                x.Roles = roles;
        });

        return null;
    }
}
