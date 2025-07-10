namespace DevSpaceWeb.Apps.Runtime.Main.Blocks;

public class SetActiveFileBlock : IActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        FileData? Category = null;
        if (Block.inputs.TryGetValue("file", out WorkspaceBlockConnection? catBlock) && catBlock.block != null)
            Category = Runtime.GetFileFromBlock(catBlock.block);

        if (Category == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set active file, could not find file.");

        Runtime.MainData.FileActive = Category;

        return null;
    }
}