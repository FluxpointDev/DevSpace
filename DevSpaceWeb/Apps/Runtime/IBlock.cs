using DevSpaceWeb.Apps.Runtime.Main;
using Discord;
using Newtonsoft.Json.Linq;
using System.Text;

namespace DevSpaceWeb.Apps.Runtime;

public class IBlock
{
    public IRuntime Runtime { get; internal set; }
    public WorkspaceBlock Block { get; internal set; }
}
public abstract class IRuntime
{
    public long UnixResponseTime;
    public DateTime CurrentResponseTime;


    public Dictionary<string, object> Variables = new Dictionary<string, object>();

    public MainData MainData = new MainData();

    public FileData? GetFileFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                    return obj as FileData;
                break;
            case "data_file_active":
                return MainData.FileActive;
            case "data_file_empty":
                return new FileData();
        }
        return null;
    }

    public JToken? SelectJsonToken(JObject json, string key)
    {
        if (string.IsNullOrEmpty(key))
            return null;

        string[] Split = key.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (Split.Length == 0)
            return null;

        JToken? Selected = json;
        foreach (string i in Split)
        {
            if (i.EndsWith(']'))
            {
                string Between = i.GetBetween("[", "]");
                if (!string.IsNullOrEmpty(Between) && int.TryParse(Between, out int number))
                {
                    if (number == -1)
                        Selected = Selected.LastOrDefault();
                    else
                        Selected = Selected.ElementAtOrDefault(number);
                }
                else
                {
                    Selected = null;
                    break;
                }
            }
            try
            {
                Selected = Selected[i];
            }
            catch
            {
                Selected = null;
                break;
            }
        }

        return Selected;
    }

    public JObject? GetJsonFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                    return obj as JObject;
                break;
            case "data_json_active":
                return MainData.JsonActive;
        }
        return null;
    }

    public abstract Task<string> GetStringFromBlock(WorkspaceBlock block);

    public async Task<string> GetBaseStringFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "color_hex_picker":
                return block.fields["custom_color"].ToString();
            case "color_hex":
                {
                    if (block.inputs.TryGetValue("hex", out WorkspaceBlockConnection? hexStringBlock) && hexStringBlock.block != null)
                    {
                        return await GetStringFromBlock(hexStringBlock.block);
                    }
                }
                break;
            case "color_rgb":
                int? R = 0;
                if (block.inputs.TryGetValue("red", out WorkspaceBlockConnection? cBlock) && cBlock.block != null)
                    R = await GetIntFromBlock(cBlock.block);

                int? G = 0;
                if (block.inputs.TryGetValue("green", out cBlock) && cBlock.block != null)
                    R = await GetIntFromBlock(cBlock.block);

                int? B = 0;
                if (block.inputs.TryGetValue("blue", out cBlock) && cBlock.block != null)
                    R = await GetIntFromBlock(cBlock.block);

                return $"{R.GetValueOrDefault(0)}, {G.GetValueOrDefault(0)}, {B.GetValueOrDefault(0)}";
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj) && obj is string str)
                    return str;
                break;
            case "text":
            case "text_multiline":
                return block.fields["TEXT"].ToString();
            case "data_json_active":
                return Newtonsoft.Json.JsonConvert.SerializeObject(MainData.JsonActive);
            case "data_selector_json":
                {
                    if (block.inputs.TryGetValue("json", out WorkspaceBlockConnection? jsonBlock) && block.inputs.TryGetValue("select", out WorkspaceBlockConnection? keyBlock) && keyBlock.block != null && jsonBlock.block != null)
                    {
                        JObject? Json = GetJsonFromBlock(jsonBlock.block);
                        string Key = await GetStringFromBlock(keyBlock.block);
                        if (Json != null)
                        {
                            JToken? Token = SelectJsonToken(Json, Key);
                            if (Token != null)
                                return Token.ToString();
                        }
                    }
                }
                break;
            case "data_string_response_time":
                TimeSpan TimeSpan = CurrentResponseTime - DateTimeOffset.FromUnixTimeSeconds(UnixResponseTime);
                string Time = "";
                if (TimeSpan.Seconds != 0)
                {
                    if (string.IsNullOrEmpty(Time))
                        Time = $"{TimeSpan.Seconds}s";
                    else
                        Time += $" {TimeSpan.Seconds}s";
                }

                if (string.IsNullOrEmpty(Time))
                    Time = $"{TimeSpan.Milliseconds}ms";
                else
                    Time += $" {TimeSpan.Milliseconds}ms";
                return Time;
            case "data_string_join":
                {
                    StringBuilder builder = new StringBuilder();
                    string Type = string.Empty;
                    if (block.fields.TryGetValue("join_type", out JToken field))
                        Type = field.ToString();

                    if (string.IsNullOrEmpty(Type))
                        throw new RuntimeError(RuntimeErrorType.Server, "Failed to run string join, invalid type");

                    int Count = 1;

                    foreach (WorkspaceBlockConnection i in block.inputs.Values)
                    {
                        if (i.block == null || !i.block.enabled)
                            continue;

                        string Text = await GetStringFromBlock(i.block);

                        if (!string.IsNullOrEmpty(Text))
                        {
                            switch (Type)
                            {
                                case "list":
                                    if (builder.Length != 0)
                                        builder.Append("\n");

                                    builder.Append("- " + Text);
                                    break;
                                case "number":
                                    if (builder.Length != 0)
                                        builder.Append("\n");

                                    builder.Append($"{Count}." + Text);
                                    break;
                                case "space":
                                    if (builder.Length != 0)
                                        builder.Append(" ");

                                    builder.Append(Text);
                                    break;
                                default:
                                    if (Type == "newline" && builder.Length != 0)
                                        builder.Append("\n");

                                    builder.Append(Text);
                                    break;
                            }
                            Count += 1;
                        }

                    }

                    if (builder.Length == 0 || string.IsNullOrEmpty(builder.ToString()))
                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to run string join, string is empty.");

                    return builder.ToString();
                }
            case "data_string_base64":
                {
                    if (block.inputs.TryGetValue("string", out WorkspaceBlockConnection? strBlock) && strBlock.block != null)
                    {
                        string Text = await GetStringFromBlock(strBlock.block);
                        if (!string.IsNullOrEmpty(Text))
                        {
                            switch (block.fields["type"].ToString())
                            {
                                case "encode":
                                    {
                                        byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Text);
                                        return System.Convert.ToBase64String(plainTextBytes);
                                    }
                                case "decode":
                                    {
                                        byte[] base64EncodedBytes = System.Convert.FromBase64String(Text);
                                        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                                    }
                            }

                        }
                    }
                }
                break;
            case "data_string_format":
                {
                    if (block.inputs.TryGetValue("string", out WorkspaceBlockConnection? strBlock) && strBlock.block != null)
                    {
                        string Text = await GetStringFromBlock(strBlock.block);
                        if (!string.IsNullOrEmpty(Text))
                        {
                            switch (block.fields["type"].ToString())
                            {
                                case "uppercase":
                                    {
                                        return Text.ToUpper();
                                    }
                                case "lowercase":
                                    {
                                        return Text.ToLower();
                                    }
                            }

                        }
                    }
                }
                break;
            case "data_string_offset":
                {
                    if (block.inputs.TryGetValue("string", out WorkspaceBlockConnection? strBlock) && strBlock.block != null)
                    {
                        string Text = await GetStringFromBlock(strBlock.block);
                        if (!string.IsNullOrEmpty(Text))
                        {
                            if ((int)block.fields["offset"] > Text.Length)
                                throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to get text offset, offset is bigger than the text given.");
                            return Text.Substring((int)block.fields["offset"]);

                        }
                    }
                }
                break;
            case "data_string_offset_end":
                {
                    if (block.inputs.TryGetValue("string", out WorkspaceBlockConnection? strBlock) && strBlock.block != null)
                    {
                        string Text = await GetStringFromBlock(strBlock.block);
                        if (!string.IsNullOrEmpty(Text))
                        {
                            if ((int)block.fields["start"] > Text.Length)
                                throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to get text offset, offset is bigger than the text given.");
                            return Text.Substring((int)block.fields["start"], (int)block.fields["end"]);

                        }
                    }
                }
                break;
            case "data_string_between":
                {
                    if (block.inputs.TryGetValue("string", out WorkspaceBlockConnection? strBlock) && strBlock.block != null)
                    {
                        string Text = await GetStringFromBlock(strBlock.block);
                        if (!string.IsNullOrEmpty(Text))
                        {
                            string Start = block.fields["start"].ToString();
                            if (string.IsNullOrEmpty(Start))
                                throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to get text between, start is missing.");

                            string End = block.fields["end"].ToString();
                            if (string.IsNullOrEmpty(End))
                                throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to get text between, end is missing.");

                            return Text.GetBetween(Start, End);

                        }
                    }
                }
                break;
            case "data_string_color":
                {
                    if (block.inputs.TryGetValue("string", out WorkspaceBlockConnection? strBlock) && strBlock.block != null)
                    {
                        string Text = await GetStringFromBlock(strBlock.block);
                        if (!string.IsNullOrEmpty(Text))
                        {
                            switch (block.fields["type"].ToString())
                            {
                                case "hex_rgb":
                                    if (Text.StartsWith('#'))
                                        Text = Text.Substring(1);

                                    if (Text.Length < 6 || Text.Length > 8)
                                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to convert to RGB, text is invalid hex format");

                                    System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(Text);
                                    if (Text.Length == 8)
                                        return $"{color.R}, {color.G}, {color.B}, {color.A}";

                                    return $"{color.R}, {color.G}, {color.B}";
                                case "rgb_hex":
                                    string[] Split = Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                                    if (Split.Length < 3 || Split.Length > 4)
                                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to convert to Hex, text is invalid rgb format");



                                    if (Split.Length == 4)
                                    {
                                        System.Drawing.Color colorAlpha = System.Drawing.Color.FromArgb(int.Parse(Split[0]), int.Parse(Split[1]), int.Parse(Split[2]), int.Parse(Split[3]));
                                        return $"#{colorAlpha.R:X2}{colorAlpha.G:X2}{colorAlpha.B:X2}{colorAlpha.A:X2}";


                                    }
                                    System.Drawing.Color colorH = System.Drawing.Color.FromArgb(int.Parse(Split[0]), int.Parse(Split[1]), int.Parse(Split[2]));
                                    return $"#{colorH.R:X2}{colorH.G:X2}{colorH.B:X2}";
                            }

                        }
                    }
                }
                break;
            case "data_string_markdown":
                {
                    if (block.inputs.TryGetValue("string", out WorkspaceBlockConnection? strBlock) && strBlock.block != null)
                    {
                        string Text = await GetStringFromBlock(strBlock.block);
                        if (!string.IsNullOrEmpty(Text))
                        {
                            switch (block.fields["type"].ToString())
                            {
                                case "heading_1":
                                    return "# " + Text;
                                case "heading_2":
                                    return "## " + Text;
                                case "heading_3":
                                    return "### " + Text;
                                case "heading_4":
                                    return "#### " + Text;
                                case "heading_5":
                                    return "##### " + Text;
                                case "heading_6":
                                    return "###### " + Text;
                                case "bold":
                                    return $"**{Text}**";
                                case "italic":
                                    return $"_{Text}_";
                                case "quote":
                                    return "> " + Text;
                                case "code":
                                    return $"`{Text}`";
                                case "code_block":
                                    return "```\n" + Text + "\n```";

                            }

                        }
                    }
                }
                break;
            case "data_string_markdown_link":
                {
                    WorkspaceBlock? nameBlock = null;
                    WorkspaceBlock? linkBlock = null;

                    if (block.inputs.TryGetValue("name", out WorkspaceBlockConnection? nb) && nb.block != null)
                        nameBlock = nb.block;

                    if (block.inputs.TryGetValue("link", out nb) && nb.block != null)
                        linkBlock = nb.block;

                    if (nameBlock == null)
                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to use markdown link, name input is missing.");

                    if (linkBlock == null)
                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to use markdown link, link input is missing.");

                    string Name = await GetStringFromBlock(nameBlock);

                    if (string.IsNullOrEmpty(Name))
                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to use markdown link, name text is missing.");

                    string Link = await GetStringFromBlock(linkBlock);

                    if (string.IsNullOrEmpty(Link))
                        throw new RuntimeError(RuntimeErrorType.Runtime, "Failed to use markdown link, Link text is missing.");

                    return $"[{Name}]({Link})";
                }
            case "data_string_markdown_code":
                {
                    if (block.inputs.TryGetValue("string", out WorkspaceBlockConnection? strBlock) && strBlock.block != null)
                    {
                        string Text = await GetStringFromBlock(strBlock.block);
                        if (!string.IsNullOrEmpty(Text))
                        {
                            return $"```{block.fields["type"].ToString()}\n" + Text + "\n```";

                        }
                    }
                }
                break;
        }
        bool? boolVal = await GetBoolFromBlock(block);
        if (boolVal.HasValue)
            return boolVal.Value.ToString();
        int? intVal = await GetIntFromBlock(block);
        if (intVal.HasValue)
            return intVal.Value.ToString();
        double? dobVal = await GetDoubleFromBlock(block);
        if (dobVal.HasValue)
            return dobVal.Value.ToString();

        if (block.type.StartsWith("data_selector_"))
        {
            object? data = MainSelectors.Parse(this, block);
            if (data != null)
                return data.ToString();
        }
        return string.Empty;
    }

    public async Task<bool?> GetBoolFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                    return (bool)obj;
                break;
            case "logic_boolean":
                return block.fields["BOOL"].ToString() == "TRUE";
            case "data_selector_json":
                {
                    if (block.inputs.TryGetValue("json", out WorkspaceBlockConnection? jsonBlock) && block.inputs.TryGetValue("select", out WorkspaceBlockConnection? keyBlock) && keyBlock.block != null && jsonBlock.block != null)
                    {
                        JObject? Json = GetJsonFromBlock(jsonBlock.block);
                        string Key = await GetStringFromBlock(keyBlock.block);
                        if (Json != null)
                        {
                            JToken? Token = SelectJsonToken(Json, Key);
                            if (Token != null)
                                return (bool?)Token;
                        }
                    }
                }
                break;
        }
        return null;
    }

    public async Task<int?> GetIntFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                    return (int)obj;
                break;
            case "math_number":
                return block.fields["NUM"].ToObject<int>();
            case "data_selector_json":
                {
                    if (block.inputs.TryGetValue("json", out WorkspaceBlockConnection? jsonBlock) && block.inputs.TryGetValue("select", out WorkspaceBlockConnection? keyBlock) && keyBlock.block != null && jsonBlock.block != null)
                    {
                        JObject? Json = GetJsonFromBlock(jsonBlock.block);
                        string Key = await GetStringFromBlock(keyBlock.block);
                        if (Json != null)
                        {
                            JToken? Token = SelectJsonToken(Json, Key);
                            if (Token != null)
                                return (int?)Token;
                        }
                    }
                }
                break;
        }

        return null;
    }

    public async Task<double?> GetDoubleFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                if (Variables.TryGetValue(block.GetVariableId(), out object? obj))
                    return (double)obj;
                break;
            case "math_number":
                return block.fields["NUM"].ToObject<double>();
            case "data_selector_json":
                {
                    if (block.inputs.TryGetValue("json", out WorkspaceBlockConnection? jsonBlock) && block.inputs.TryGetValue("select", out WorkspaceBlockConnection? keyBlock) && keyBlock.block != null && jsonBlock.block != null)
                    {
                        JObject? Json = GetJsonFromBlock(jsonBlock.block);
                        string Key = await GetStringFromBlock(keyBlock.block);
                        if (Json != null)
                        {
                            JToken? Token = SelectJsonToken(Json, Key);
                            if (Token != null)
                                return (double?)Token;
                        }
                    }
                }
                break;
        }

        return null;
    }

    public object? GetVariableFromBlock(WorkspaceBlock block)
    {
        if (block.type == "variables_get" && Variables.TryGetValue(block.GetVariableId(), out object? obj))
            return (HttpResponseMessage)obj;

        return null;
    }

    public ResponseData? GetResponseFromBlock(WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "variables_get":
                return (ResponseData)GetVariableFromBlock(block);
            case "data_response_active":
                return MainData.ResponseActive;
        }
        return null;
    }


    public void SetVariable(string key, object value)
    {
        if (Variables.ContainsKey(key))
            Variables[key] = value;
        else
            Variables.TryAdd(key, value);
    }

    public void SetFileData(WorkspaceBlock block, FileData file)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, file);
                break;
            case "data_file_active":
                MainData.FileActive = file;
                break;
        }
    }

    public void SetFileData(WorkspaceBlock block, Attachment attach)
    {
        SetFileData(block, new FileData
        {
            Name = attach.Filename.Split('.').First(),
            Url = attach.Url,
            Mime = attach.ContentType
        });
    }

    public void SetResponse(WorkspaceBlock block, ResponseData res)
    {
        switch (block.type)
        {
            case "variables_get":
                string Key = block.GetVariableId();
                SetVariable(Key, res);
                break;
            case "data_response_active":
                MainData.ResponseActive = res;
                break;
        }
    }
}
public abstract class IActionBlock : IBlock
{
    public abstract Task<RuntimeError?> RunAsync();
}
public class RuntimeError(RuntimeErrorType type, string? error) : Exception(error)
{
    public RuntimeErrorType Type = type;
    public string? ErrorMessage = error;
    public object? CustomMessage;
}
public enum RuntimeErrorType
{
    Runtime, Request, Discord, Server, StopExecution
}
