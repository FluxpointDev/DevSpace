namespace DevSpaceWeb.Apps.Runtime.Main;

public static class MainSelectors
{
    public static object? Parse(IRuntime runtime, WorkspaceBlock block)
    {
        switch (block.type)
        {
            case "data_selector_file":
                {
                    if (block.inputs.TryGetValue("file", out WorkspaceBlockConnection? fileBlock) && fileBlock.block != null)
                    {
                        FileData? file = runtime.GetFileFromBlock(fileBlock.block);
                        if (file == null)
                            return null;

                        switch (block.fields["property"].ToString())
                        {
                            case "file_name":
                                return file.Name;
                            case "file_name_extension":
                                return file.Name + file.Mime.Split('/').Last();
                            case "type":
                                return file.Mime.Split('/').Last();
                            case "mime_type":
                                return file.Mime;

                        }
                    }
                }
                break;
            case "data_selector_response":
                {
                    if (block.inputs.TryGetValue("response", out WorkspaceBlockConnection? resBlock) && resBlock.block != null)
                    {
                        ResponseData? response = runtime.GetResponseFromBlock(resBlock.block);
                        if (response == null)
                            return null;

                        switch (block.fields["property"].ToString())
                        {
                            case "is_success":
                                return response.IsSuccess;
                            case "status_code":
                                return response.StatusCode;
                            case "has_content":
                                return response.ContentLength != 0;
                            case "content_length":
                                return response.ContentLength;
                            case "mime_type":
                                return response.ContentType;

                        }
                    }
                }
                break;
        }

        return null;
    }
}