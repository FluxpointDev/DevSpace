using Discord;
using Discord.Net;
using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;

public class RemoveReactionBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestMessage? Message = null;
        if (Block.inputs.TryGetValue("message", out WorkspaceBlockConnection? messageBlock) && messageBlock.block != null)
            Message = await Runtime.GetMessageFromBlock(messageBlock.block) as RestUserMessage;

        if (Message == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to remove reaction from message, could not find message.");

        IEmote? EmojiObject = null;
        if (Block.inputs.TryGetValue("emoji", out WorkspaceBlockConnection? emojiBlock) && emojiBlock.block != null)
        {
            Tuple<Tuple<ulong, RestGuild?>?, IEmote>? Emotes = await Runtime.GetEmojiFromBlockAsync(emojiBlock.block);
            if (Emotes == null || Emotes.Item2 == null)
                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to add reaction to message, emoji could not be parsed.");

            EmojiObject = Emotes.Item2;
        }

        RestGuildUser? Member = null;
        if (Block.inputs.TryGetValue("member", out WorkspaceBlockConnection? memberBlock) && memberBlock.block != null)
            Member = await Runtime.GetMemberFromBlock(memberBlock.block);

        switch (Block.type)
        {
            case "action_remove_reaction_self":
                {
                    if (EmojiObject == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to remove reaction from app message, could not parse emoji.");

                    try
                    {
                        await Message.RemoveReactionAsync(EmojiObject, Client.CurrentUser);
                    }
                    catch (HttpException http) when (http.DiscordCode == DiscordErrorCode.UnknownEmoji)
                    {
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to remove reaction from app message, unknown emoji or bot does not have access to the emoji.");
                    }
                }

                break;
            case "action_remove_reaction_member":
                {
                    if (Member == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to remove reaction from member message, could not find member.");

                    if (EmojiObject == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to remove reaction from member message, could not parse emoji.");

                    try
                    {
                        await Message.RemoveReactionAsync(EmojiObject, Member);
                    }
                    catch (HttpException http) when (http.DiscordCode == DiscordErrorCode.UnknownEmoji)
                    {
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to remove reaction from member message, unknown emoji or bot does not have access to the emoji.");
                    }
                }

                break;
            case "action_remove_all_reactions_member":
                {
                    if (Member == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to remove all reactions from member message, could not find member.");

                    foreach (KeyValuePair<IEmote, ReactionMetadata> i in Message.Reactions)
                    {
                        try
                        {
                            await Message.RemoveReactionAsync(i.Key, Member);
                        }
                        catch { }
                    }
                }

                break;
            case "action_remove_reactions":
                {
                    if (EmojiObject == null)
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to remove all reactions from message, could not parse emoji.");

                    try
                    {
                        await Message.RemoveAllReactionsForEmoteAsync(EmojiObject);
                    }
                    catch (HttpException http) when (http.DiscordCode == DiscordErrorCode.UnknownEmoji)
                    {
                        return new RuntimeError(RuntimeErrorType.Runtime, "Failed to remove all reactions from message, unknown emoji or bot does not have access to the emoji.");
                    }
                }

                break;
            case "action_remove_all_reactions":
                {
                    await Message.RemoveAllReactionsAsync();
                }

                break;
            default:
                return new RuntimeError(RuntimeErrorType.Server, "Failed to remove reaction, server could not run invalid action type.");

        }
        

        return null;
    }
}
