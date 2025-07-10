using Newtonsoft.Json.Linq;

namespace DevSpaceWeb.Apps.Runtime.Main.Blocks;

public class RequestObjectBlock : IBlock
{
    public async Task<string> Url()
    {
        if (Block.inputs.TryGetValue("url", out WorkspaceBlockConnection? content) && content.block != null)
            return await Runtime.GetStringFromBlock(content.block);
        return string.Empty;
    }

    public async Task<bool> Fail()
    {
        if (Block.inputs.TryGetValue("is_fail", out WorkspaceBlockConnection? content) && content.block != null)
            return (await Runtime.GetBoolFromBlock(content.block)).GetValueOrDefault(false);
        return false;
    }

    public HttpMethod Type()
    {
        switch (Block.fields["api_type"].ToString())
        {
            case "get":
                return HttpMethod.Get;
            case "post":
                return HttpMethod.Post;
            case "put":
                return HttpMethod.Put;
            case "patch":
                return HttpMethod.Patch;
            case "delete":
                return HttpMethod.Delete;
        }

        return null;
    }

    public async Task<Dictionary<string, string>> Headers()
    {
        Dictionary<string, string> list = new Dictionary<string, string>();
        if (Block.inputs.TryGetValue("obj_api_headers_list", out WorkspaceBlockConnection? headerBlock) && headerBlock.block != null)
        {
            foreach (KeyValuePair<string, WorkspaceBlockConnection> i in headerBlock.block.inputs)
            {
                if (i.Value.block != null && i.Value.block.inputs.TryGetValue("value", out WorkspaceBlockConnection? itemBlock) && itemBlock.block != null)
                {
                    if (i.Value.block.type == "obj_api_list_auth")
                        list.Add("Authorization", await Runtime.GetStringFromBlock(itemBlock.block));
                    else if (i.Value.block.type == "obj_api_list_item")
                        list.Add(i.Value.block.fields["key"].ToString(), await Runtime.GetStringFromBlock(itemBlock.block));
                }
            }
        }
        return list;
    }

    public async Task<Dictionary<string, string>> Query()
    {
        Dictionary<string, string> list = new Dictionary<string, string>();
        if (Block.inputs.TryGetValue("obj_api_query_list", out WorkspaceBlockConnection? queryBlock) && queryBlock.block != null)
        {
            foreach (KeyValuePair<string, WorkspaceBlockConnection> i in queryBlock.block.inputs)
            {
                if (i.Value.block != null && i.Value.block.inputs.TryGetValue("value", out WorkspaceBlockConnection? itemBlock) && itemBlock.block != null)
                {
                    if (i.Value.block.type == "obj_api_list_auth")
                        list.Add("Authorization", await Runtime.GetStringFromBlock(itemBlock.block));
                    else if (i.Value.block.type == "obj_api_list_item")
                        list.Add(i.Value.block.fields["key"].ToString(), await Runtime.GetStringFromBlock(itemBlock.block));
                }
            }
        }
        return list;
    }

    public async Task<object?> Body()
    {
        string Value = string.Empty;
        if (Block.inputs.TryGetValue("body", out WorkspaceBlockConnection? content) && content.block != null)
        {
            if (content.block.type == "data_file_active")
                return Runtime.MainData.FileActive;
            else if (content.block.type == "data_json_active")
                return Runtime.MainData.JsonActive;
            else if (content.block.type == "obj_api_json_list")
            {
                JObject create = new JObject();
                foreach (WorkspaceBlockConnection i in content.block.inputs.Values)
                {
                    RuntimeError? error = await ParseJsonKeys(create, i);
                    if (error != null)
                        throw error;
                }
                return create;
            }
            else
                Value = await Runtime.GetStringFromBlock(content.block);
        }

        Console.WriteLine("Request body: " + Value);
        return Value;
    }

    public async Task<RuntimeError?> ParseJsonKeys(JObject json, WorkspaceBlockConnection block)
    {
        if (block.block.type == "obj_api_list_item")
        {
            WorkspaceBlock? Input = block.block.inputs.First().Value.block;
            if (Input.type == "obj_api_json_list")
            {
                JObject create = new JObject();
                foreach (WorkspaceBlockConnection i in Input.inputs.Values)
                {
                    RuntimeError? error = await ParseJsonKeys(create, i);
                    if (error != null)
                        return error;
                }
                if (json.ContainsKey(block.block.fields["key"].ToString()))
                    return new RuntimeError(RuntimeErrorType.Runtime, "Json list already has the key " + block.block.fields["key"].ToString());

                json.Add(block.block.fields["key"].ToString(), create);
            }
            else
            {
                string Value = await Runtime.GetStringFromBlock(Input);

                if (json.ContainsKey(block.block.fields["key"].ToString()))
                    return new RuntimeError(RuntimeErrorType.Runtime, "Json list already has the key " + block.block.fields["key"].ToString());

                json.Add(block.block.fields["key"].ToString(), Value);
            }

        }

        return null;
    }
}
