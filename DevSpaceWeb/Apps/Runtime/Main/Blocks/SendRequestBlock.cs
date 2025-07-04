using Newtonsoft.Json.Linq;

namespace DevSpaceWeb.Apps.Runtime.Main.Blocks;

public class SendRequestBlock : IActionBlock
{
    public override async Task<RuntimeError?> RunAsync()
    {
        RequestObjectBlock? RequestBlock = null;

        if (Block.inputs.TryGetValue("request", out RequestBlocksBlock? webBlock) && webBlock.block != null)
            RequestBlock = MainBlocks.Parse(Runtime, webBlock.block) as RequestObjectBlock;

        if (RequestBlock == null)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send request, request object data is missing.");

        string Url = await RequestBlock.Url();

        foreach (KeyValuePair<string, string> i in await RequestBlock.Query())
        {
            if (Url.Contains('?'))
                Url += $"&{Uri.EscapeDataString(i.Key)}={Uri.EscapeDataString(i.Value)}";
            else
                Url += $"?{Uri.EscapeDataString(i.Key)}={Uri.EscapeDataString(i.Value)}";
        }

        if (string.IsNullOrEmpty(Url))
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send request, request url is missing.");

        if (!Url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send request, request url needs to use https://");

        if (!Uri.TryCreate(Url, UriKind.Absolute, out Uri? uri))
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send request, request url format is invalid.");

        if (!uri.Host.Contains(".") || uri.HostNameType != UriHostNameType.Dns)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send request, request url needs to be a domain.");
        if (uri.IsLoopback)
            return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send request, request url can't use localhost.");

        object? Body = await RequestBlock.Body();

        HttpRequestMessage Req = new HttpRequestMessage(RequestBlock.Type(), Url);
        if (Body != null)
        {
            Console.WriteLine("Got body: " + Body.GetType().Name);
            if (Body is FileData fd)
                Req.Content = new ByteArrayContent(fd.Buffer);
            else if (Body is JObject jo)
                Req.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(jo), System.Text.Encoding.UTF8, "application/json");
            else if (Body is string str)
            {
                if (!string.IsNullOrEmpty(str) && (str.StartsWith('{') || str.StartsWith('[')))
                    Req.Content = new StringContent(str, System.Text.Encoding.UTF8, "application/json");
                else
                {
                    Console.WriteLine("Set String");
                    Req.Content = new StringContent(str, System.Text.Encoding.UTF8);
                }
            }
        }

        foreach (KeyValuePair<string, string> i in await RequestBlock.Headers())
        {
            if (Req.Headers.Contains(i.Key))
            {
                Req.Headers.Remove(i.Key);
                Req.Headers.Add(i.Key, i.Value);
            }
            else
                Req.Headers.Add(i.Key, i.Value);
        }


        HttpResponseMessage Res = await Program.RuntimeHttpClient.SendAsync(Req);
        if (await RequestBlock.Fail() && !Res.IsSuccessStatusCode)
            return new RuntimeError(RuntimeErrorType.Runtime, $"Failed to send request, response code is {Res.StatusCode} - {Res.ReasonPhrase ?? "Error"}.");

        if (RequestBlock.Block.inputs.TryGetValue("response", out RequestBlocksBlock? resBlock) && resBlock.block != null)
        {
            Runtime.SetResponse(resBlock.block, new ResponseData
            {
                IsSuccess = Res.IsSuccessStatusCode,
                StatusCode = (int)Res.StatusCode,
                ContentLength = Res.Content?.Headers.ContentLength.GetValueOrDefault(0) ?? 0,
                ContentType = Res.Content?.Headers.ContentType?.MediaType ?? string.Empty,
                Headers = Res.Headers
            });
        }

        if (!string.IsNullOrEmpty(Res.Content.Headers.ContentType?.MediaType) && RequestBlock.Block.inputs.TryGetValue("output_data", out RequestBlocksBlock? hookBlock) && hookBlock.block != null)
        {
            RequestBlocks_Block? OutputResponse = hookBlock.block;
            switch (Res.Content.Headers.ContentType.MediaType)
            {
                case "text/plain":
                    {
                        switch (OutputResponse.type)
                        {
                            case "variables_get":
                                string Key = OutputResponse.GetVariableId();
                                string Content = await Res.Content.ReadAsStringAsync();
                                Runtime.SetVariable(Key, Content);
                                break;
                            default:
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send request, response output could not be set with the block type.");
                        }
                    }
                    break;
                case "image/jpeg":
                case "image/jpg":
                case "image/png":
                case "image/webp":
                    {
                        switch (OutputResponse.type)
                        {
                            case "variables_get":
                                {
                                    string Key = OutputResponse.GetVariableId();
                                    byte[] Bytes = await Res.Content.ReadAsByteArrayAsync();

                                    Runtime.SetVariable(Key, new FileData() { Buffer = Bytes, Mime = Res.Content.Headers.ContentType.MediaType, Name = "image" });
                                }
                                break;
                            case "data_file_active":
                                {
                                    byte[] Bytes = await Res.Content.ReadAsByteArrayAsync();

                                    Runtime.MainData.FileActive = new FileData() { Buffer = Bytes, Mime = Res.Content.Headers.ContentType.MediaType, Name = "image" };
                                }
                                break;
                            default:
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send request, response output could not be set with the block type.");
                        }
                    }
                    break;
                case "application/json":
                    {
                        switch (OutputResponse.type)
                        {
                            case "variables_get":
                                {
                                    string Key = OutputResponse.GetVariableId();
                                    string Content = await Res.Content.ReadAsStringAsync();

                                    Runtime.SetVariable(Key, JObject.Parse(Content));
                                }
                                break;
                            case "data_json_active":
                                {
                                    string Content = await Res.Content.ReadAsStringAsync();
                                    Runtime.MainData.JsonActive = JObject.Parse(Content);
                                }
                                break;
                            default:
                                return new RuntimeError(RuntimeErrorType.Runtime, "Failed to send request, response output could not be set with the block type.");
                        }
                    }
                    break;
                default:
                    return new RuntimeError(RuntimeErrorType.Runtime, $"Failed to send request, response content type {Res.Content.Headers.ContentType.MediaType} is not allowed.");
            }
        }

        return null;
    }
}
