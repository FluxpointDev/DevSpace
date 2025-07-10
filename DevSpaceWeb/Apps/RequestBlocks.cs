using Newtonsoft.Json.Linq;

namespace DevSpaceWeb.Apps;

public class WorkspaceRequest
{
    public WorkspaceBlocks blocks = new WorkspaceBlocks();
    public List<WorkspaceVariable> variables = new List<WorkspaceVariable>();
}
public class WorkspaceBlocks
{
    public List<WorkspaceBlock> blocks = new List<WorkspaceBlock>();
}
public class WorkspaceBlockConnection
{
    public WorkspaceBlock? block = null;

    public bool TryGetInput(string key, out WorkspaceBlock? data)
    {
        if (block != null && block.inputs.TryGetValue(key, out WorkspaceBlockConnection? inputBlock))
        {
            data = inputBlock.block;
            return true;
        }
        data = null;
        return false;
    }

    public bool TryGetField(string key, out JToken? token)
    {
        if (block != null && block.fields.TryGetValue(key, out token))
            return true;

        token = null;
        return false;
    }

    public bool TryGetField(int index, out JToken token)
    {
        if (block != null)
        {
            try
            {
                KeyValuePair<string, JToken> element = block.fields.ElementAt(index);
                token = element.Value;
                return true;
            }
            catch { }
        }
        token = null;
        return false;
    }
}
public class WorkspaceBlock
{
    public string type;
    public string id;
    public WorkspaceBlockIcons? icons;
    public string[]? disabledReasons = null;
    public bool enabled => disabledReasons == null || disabledReasons.Length == 0;
    public Dictionary<string, object>? extraState = null;
    public Dictionary<string, JToken> fields = new Dictionary<string, JToken>();
    public Dictionary<string, WorkspaceBlockConnection> inputs = new Dictionary<string, WorkspaceBlockConnection>();
    public WorkspaceBlockConnection? next = null;
    public double x;
    public double y;

    public string GetVariableId()
    {
        return fields.First().Value.Value<string>("id");
    }
}
public class WorkspaceBlockIcons
{
    public WorkspaceBlockComment? comment;
}
public class WorkspaceBlockComment
{
    public string? text;
    public bool pinned;
    public int height;
    public int width;
    public double x;
    public double y;
}
public class WorkspaceVariable
{
    public string name;
    public string id;
}