using Microsoft.AspNetCore.Components;

namespace DevSpaceWeb.Components.Markdown;

public class MarkdownBase : ComponentBase
{
    protected string? ContentValue { get; set; }

    protected void TextChanged(ChangeEventArgs e)
    {
        if (e.Value != null)
            ContentValue = e.Value.ToString();
        else
            ContentValue = "";
    }
}