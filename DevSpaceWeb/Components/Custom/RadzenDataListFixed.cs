using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace DevSpaceWeb.Components.Custom;

public partial class RadzenDataListFixed<TItem> : RadzenDataList<TItem>
{
    protected override async Task OnPageSizeChanged(int value)
    {
        await base.OnPageSizeChanged(value);
        await PageSizeChanged.InvokeAsync(value);
    }

    [Parameter]
    public EventCallback<int> PageSizeChanged { get; set; }
}
