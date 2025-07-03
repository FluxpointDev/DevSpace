using Newtonsoft.Json.Linq;

namespace DevSpaceWeb.Apps;

public class RequestBlocks
{
    public RequestBlocksInner blocks = new RequestBlocksInner();
    public List<RequestBlocks_Variable> variables = new List<RequestBlocks_Variable>();
}
public class RequestBlocksInner
{
    public List<RequestBlocks_Block> blocks = new List<RequestBlocks_Block>();
}
public class RequestBlocksBlock
{
    public RequestBlocks_Block? block = null;
    public RequestBlocks_Block? shadow = null;

    public bool TryGetInput(string key, out RequestBlocks_Block? data)
    {
        if (block != null && block.inputs.TryGetValue(key, out RequestBlocksBlock? inputBlock))
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
public class RequestBlocksNext
{
    public RequestBlocks_Block? block = null;
    public RequestBlocks_Block? shadow = null;
}
public class RequestBlocks_Block
{
    public string type;
    public string id;
    public RequestBlocks_Icons? icons;
    public string[]? disabledReasons = null;
    public bool enabled => disabledReasons == null || disabledReasons.Length == 0;
    public Dictionary<string, object>? extraState = null;
    public Dictionary<string, JToken> fields = new Dictionary<string, JToken>();
    public Dictionary<string, RequestBlocksBlock> inputs = new Dictionary<string, RequestBlocksBlock>();
    public RequestBlocksNext? next = null;

    public string GetVariableId()
    {
        return fields.First().Value.Value<string>("id");
    }
}
public class RequestBlocks_Icons
{
    public RequestBlocks_Comment? comment;
}
public class RequestBlocks_Comment
{
    public string? text;
    public bool pinned;
    public int height;
    public int width;
    public double x;
    public double y;
}
public class RequestBlocks_Variable
{
    public string name;
    public string id;
}