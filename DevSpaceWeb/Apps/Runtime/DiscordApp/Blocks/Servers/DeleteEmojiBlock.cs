using Discord;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Servers;

public class DeleteEmojiBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        Tuple<Tuple<ulong, RestGuild?>?, IEmote>? Emoji = null;

        if (Block.inputs.TryGetValue("emoji", out WorkspaceBlockConnection? chanBlock) && chanBlock.block != null)
            Emoji = await Runtime.GetEmojiFromBlockAsync(chanBlock.block);

        if (Emoji == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to delete emoji, could not find emoji.");

        if (Emoji.Item2 is not GuildEmote ge)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to delete emoji, this emoji is not a server emoji.");

        if (Emoji.Item1 == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to delete emoji, could not find server.");

        if (Emoji.Item1.Item1 == Runtime.Interaction.GuildId.GetValueOrDefault() && !Runtime.Interaction.Permissions!.Value.ManageEmojisAndStickers)
            return Runtime.GetAppPermissionError(GuildPermission.ManageEmojisAndStickers);

        if (Emoji.Item1 == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify emoji, this is not a server emoji.");

        RestGuild Guild = Emoji.Item1.Item2;
        if (Guild == null)
            Guild = await Runtime.GetGuildAsync(Emoji.Item1.Item1);

        if (Guild == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to modify emoji, could not find Discord server.");

        await Guild.DeleteEmoteAsync(Emoji.Item2 as GuildEmote);

        return null;
    }
}
