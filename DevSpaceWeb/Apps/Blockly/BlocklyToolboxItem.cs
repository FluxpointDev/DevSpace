using Newtonsoft.Json;

namespace DevSpaceWeb.Apps;

public class BlocklyToolboxItem
{
    public string kind = "";

    [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? name;

    [JsonProperty("colour", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? colour;

    [JsonProperty("icon", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? icon;

    [JsonProperty("text", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? text;

    [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? type;

    [JsonProperty("custom", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? custom;

    [JsonProperty("categorystyle", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? categorystyle;

    [JsonProperty("movable", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool? movable;

    [JsonProperty("deletable", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool? deletable;

    [JsonProperty("editable", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool? editable;

    [JsonProperty("expanded", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool? expanded;

    [JsonProperty("gap", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? gap;

    [JsonProperty("contents", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<BlocklyToolboxItem>? contents;

    [JsonProperty("inputs", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Dictionary<string, BlocklyToolboxInput>? inputs;

    public static BlocklyToolboxItem Create(WorkspaceRequest blocks)
    {
        BlocklyToolboxItem item = new BlocklyToolboxItem
        {
            kind = "categoryToolbox",
            contents = new List<BlocklyToolboxItem>
            {
                new BlocklyToolboxItem
                {
                    kind = "search",
                    name = "Search"
                }
            }
        };
        foreach (WorkspaceBlock b in blocks.blocks.blocks)
        {
            if (item.contents == null)
                item.contents = new List<BlocklyToolboxItem>();
            ParseBlock(b, item.contents, true);

        }
        //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(item, Formatting.Indented, new JsonSerializerSettings
        //{
        //    NullValueHandling = NullValueHandling.Ignore
        //}));
        return item;
    }

    public static Dictionary<string, BlocklyToolboxInput> ParseNextInput(KeyValuePair<string, WorkspaceBlockConnection> input)
    {
        Dictionary<string, BlocklyToolboxInput> dict = new Dictionary<string, BlocklyToolboxInput>();
        if (input.Value.block != null)
        {
            ParseInput(dict, input.Value.block);
        }
        return dict;
    }

    public static void ParseInput(Dictionary<string, BlocklyToolboxInput> parent, WorkspaceBlock input)
    {
        BlocklyToolboxItem item = new BlocklyToolboxItem();
        ParseBlock(item, input.inputs.First().Value.block);

        if (input.fields["shadow"].ToString() == "False")
        {
            parent.Add(input.fields["name"].ToString(), new BlocklyToolboxInput
            {
                block = item
            });
        }
        else
        {
            parent.Add(input.fields["name"].ToString(), new BlocklyToolboxInput
            {
                shadow = item
            });
        }
        if (input.next != null && input.next.block != null)
            ParseInput(parent, input.next.block);
    }

    public static void ParseBlock(BlocklyToolboxItem item, WorkspaceBlock block)
    {
        string BlockType = "";
        try
        {
            BlockType = block.fields["type"].ToString();
        }
        catch { }
        string BlockEditable = block.fields["editable"].ToString();
        string BlockMovable = block.fields["movable"].ToString();
        string BlockDeletable = block.fields["deletable"].ToString();

        item.kind = "block";
        item.type = BlockType;
        if (BlockEditable == "False")
            item.editable = false;
        if (BlockMovable == "False")
            item.movable = false;
        if (BlockDeletable == "False")
            item.deletable = false;

        if (block.inputs != null && block.inputs.Count != 0)
            item.inputs = ParseNextInput(block.inputs.First());
    }

    public static BlocklyToolboxItem ParseBlock(WorkspaceBlock block, List<BlocklyToolboxItem> parent, bool firstRun = false, bool firstInput = false)
    {
        BlocklyToolboxItem item = new BlocklyToolboxItem();
        switch (block.type)
        {
            case "blockly_toolbox_category":
                string CatName = block.fields["name"].ToString();
                string CatColor = block.fields["color"].ToString();
                string CatIcon = block.fields["icon"].ToString();
                string CatExpand = block.fields["expanded"].ToString();

                item.kind = "category";
                if (CatName == "Variables")
                {
                    item.name = "Variables";
                    item.custom = "VARIABLE";
                    item.categorystyle = "variable_category";
                    item.icon = "mdi:variable";
                }
                else
                {
                    item.name = CatName;

                    if (!string.IsNullOrEmpty(CatColor))
                    {
                        if (int.TryParse(CatColor, out _) || CatColor.StartsWith('#'))
                            item.colour = CatColor;
                        else
                            item.categorystyle = CatColor;
                    }

                    if (!string.IsNullOrEmpty(CatIcon))
                        item.icon = CatIcon;

                    if (CatExpand.Equals("True"))
                        item.expanded = true;

                    foreach (KeyValuePair<string, WorkspaceBlockConnection> i in block.inputs)
                    {
                        if (item.contents == null)
                            item.contents = new List<BlocklyToolboxItem>();

                        ParseBlock(i.Value.block, item.contents, true);
                    }

                }
                break;
            case "blockly_toolbox_seperator":
                item.kind = "sep";
                item.gap = 16;
                break;
            case "blockly_toolbox_label":
                item.kind = "label";
                item.text = "   " + block.fields["text"].ToString();
                break;
            case "blockly_toolbox_block":
                ParseBlock(item, block);

                if (block.inputs != null && block.inputs.Count != 0)
                {
                    item.inputs = ParseNextInput(block.inputs.First());

                }

                if (parent == null)
                    return item;

                break;
        }


        if (firstRun && parent != null)
            parent.Add(item);

        if (block.next != null)
        {
            if (block.next.block != null)
                ParseBlock(block.next.block, parent, true);

        }

        if (string.IsNullOrEmpty(item.kind))
            return null;

        return item;
    }
}
public class BlocklyToolboxInput
{
    [JsonProperty("block", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public BlocklyToolboxItem? block;

    [JsonProperty("shadow", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public BlocklyToolboxItem? shadow;
}
