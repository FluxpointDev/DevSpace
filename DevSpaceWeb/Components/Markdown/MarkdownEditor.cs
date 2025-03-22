using Microsoft.AspNetCore.Components;
using System.Timers;

namespace DevSpaceWeb.Components.Markdown;

public class EditorBase : ComponentBase, IDisposable
{
    private System.Timers.Timer debounceTimer = new System.Timers.Timer()
    {
        Interval = 1000,
        AutoReset = false
    };

    [Parameter] public string Bindvalue { get; set; } = "";

    private object finalValue;

    [Parameter] public EventCallback<ChangeEventArgs> OnChange { get; set; }

    protected void InputChanged(string text)
    {
        finalValue = text;
        if (!debounceTimer.Enabled)
        {
            debounceTimer.Elapsed += DebounceTimer_Elapsed;
            debounceTimer.Start();
        }
        else
        {
            ExtendDebounceTimer();
        }
    }

    private async void DebounceTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        await InvokeAsync(() => OnChange.InvokeAsync(new ChangeEventArgs { Value = finalValue }));
    }

    public void Dispose()
    {
        debounceTimer.Elapsed -= DebounceTimer_Elapsed;
        debounceTimer.Dispose();
    }

    private void ExtendDebounceTimer()
    {
        debounceTimer.Stop();
        debounceTimer.Start();
    }
}