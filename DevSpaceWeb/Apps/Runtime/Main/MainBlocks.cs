using DevSpaceWeb.Apps.Runtime.Main.Blocks;

namespace DevSpaceWeb.Apps.Runtime.Main;

public static class MainBlocks
{
    public static IBlock? Parse(IRuntime runtime, RequestBlocks_Block data)
    {
        if (!data.enabled)
            return null;

        IBlock? Block = null;
        try
        {
            switch (data.type)
            {
                case "obj_api":
                    Block = new RequestObjectBlock();
                    break;
            }
        }
        catch { }

        if (Block != null)
        {
            Block.Block = data;
            Block.Runtime = runtime;
        }

        return Block;
    }

    public static IActionBlock? ParseAction(IRuntime runtime, RequestBlocks_Block data)
    {
        if (!data.enabled)
            return null;

        IActionBlock? Block = null;
        try
        {
            switch (data.type)
            {
                //
                // Channels
                //
                case "action_file_set_name":
                    Block = new SetFileNameBlock();
                    break;
                case "action_set_active_file":
                    Block = new SetActiveFileBlock();
                    break;
                case "action_api":
                    Block = new SendRequestBlock();
                    break;
                case "variables_set":
                    Block = new SetVariableBlock();
                    break;
            }
        }
        catch { }

        if (Block != null)
        {
            Block.Block = data;
            Block.Runtime = runtime;
        }


        return Block;
    }
}
