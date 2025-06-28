using Discord.Rest;

namespace DevSpaceWeb.Apps.Runtime.DiscordApp;

public class DiscordBlock : IBlock
{
    public void Set(DiscordRuntime runtime, DiscordRestClient client, RequestBlocks_Block block)
    {
        Runtime = runtime;
        Client = client;
        Block = block;
    }

    public DiscordRuntime Runtime { get; internal set; }
    public DiscordRestClient Client { get; internal set; }
}
public abstract class DiscordActionBlock : IActionBlock
{
    public void Set(DiscordRuntime runtime, DiscordRestClient client, RequestBlocks_Block block)
    {
        Runtime = runtime;
        Client = client;
        Block = block;
    }

    public new DiscordRuntime Runtime { get; internal set; }
    public DiscordRestClient Client { get; internal set; }


}
