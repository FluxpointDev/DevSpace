namespace DevSpaceWeb.Apps.Runtime.DiscordApp.Blocks.Messages;

public class DeleteInteractionMessageBlock : DiscordActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        await Runtime.Interaction.DeleteOriginalResponseAsync();

        return null;
    }
}
