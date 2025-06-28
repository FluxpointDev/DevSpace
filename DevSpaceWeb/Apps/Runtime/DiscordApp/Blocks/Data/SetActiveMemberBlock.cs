using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Data;

public class SetActiveMemberBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RestGuildUser? Member = null;
        if (Block.inputs.TryGetValue("member", out RequestBlocksBlock? catBlock) && catBlock.block != null)
            Member = await Runtime.GetMemberFromBlock(catBlock.block);

        if (Member == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set active member, could not find member.");

        Runtime.Data.MemberActive = Tuple.Create<DiscordCachableMember, RestGuildUser?>(new DiscordCachableMember(Member.GuildId, Member.Id), Member);

        return null;
    }
}