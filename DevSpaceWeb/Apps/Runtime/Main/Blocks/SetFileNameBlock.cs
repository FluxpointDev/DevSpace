namespace DevSpaceWeb.Apps.Runtime.Main.Blocks;

public class SetFileNameBlock : IActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        string Name = string.Empty;
        FileData? File = null;

        if (Block.inputs.TryGetValue("file", out RequestBlocksBlock? fileBlock) && fileBlock.block != null)
            File = Runtime.GetFileFromBlock(fileBlock.block);

        if (File == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set file name, file is missing.");

        if (Block.inputs.TryGetValue("name", out RequestBlocksBlock? channelBlock) && channelBlock.block != null)
            Name = await Runtime.GetStringFromBlock(channelBlock.block);

        if (string.IsNullOrEmpty(Name))
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to set file name, name is empty.");

        File.Name = Name;

        return null;

    }
}
