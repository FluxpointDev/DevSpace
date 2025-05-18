using Microsoft.AspNetCore.Components;

namespace DevSpaceWeb;

[EventHandler("oncustompaste", typeof(PasteTextEventsArgs),
    enableStopPropagation: true, enablePreventDefault: true)]
public static class EventHandlers
{
}

public class PasteTextEventsArgs : EventArgs
{
    public string? Text { get; set; }

    public string? GetCode(int length)
    {
        if (string.IsNullOrEmpty(Text))
            return null;

        Text = Text.Trim().Replace(" ", "");
        if (!string.IsNullOrEmpty(Text) && Text.Length > length)
            Text = Text.Substring(0, length);

        return Text;
    }
}