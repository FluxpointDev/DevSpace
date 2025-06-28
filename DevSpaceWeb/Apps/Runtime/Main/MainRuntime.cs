namespace DevSpaceWeb.Apps.Runtime.Main;

public class MainRuntime : IRuntime
{
    public override async Task<string> GetStringFromBlock(RequestBlocks_Block block)
    {
        return await base.GetBaseStringFromBlock(block);
    }
}
