using Microsoft.AspNetCore.Components;
using System.Text;

namespace DevSpaceWeb.Components.Custom;

public partial class RadzenTextMask
{
    [Parameter]
    public string? Value { get; set; }

    [Parameter, EditorRequired]
    public RadzenTextMaskType Type { get; set; }

    [Parameter]
    public bool ShowCopy { get; set; }

    private bool IsVisible { get; set; }

    public string? GetMask()
    {
        if (string.IsNullOrEmpty(Value))
            return Value;

        switch (Type)
        {
            case RadzenTextMaskType.IP:
                {
                    bool FirstDot = false;
                    StringBuilder result = new StringBuilder();
                    foreach (char c in Value)
                    {
                        if (c == '.')
                        {
                            result.Append(c);
                            FirstDot = true;
                        }
                        else
                        {
                            if (FirstDot)
                                result.Append('*');
                            else
                                result.Append(c);
                        }
                    }
                    return result.ToString();
                }
            case RadzenTextMaskType.Email:
                {
                    StringBuilder result = new StringBuilder();
                    bool AtSignDone = false;
                    foreach (char c in Value)
                    {
                        if (AtSignDone)
                        {
                            result.Append(c);
                        }
                        else
                        {
                            if (c == '@')
                            {
                                AtSignDone = true;
                                result.Append(c);
                            }
                            else
                                result.Append('*');
                        }
                    }
                    return result.ToString();
                }
            case RadzenTextMaskType.Protocol:
                {
                    string Between = Value.GetBetween("https://", "@");
                    return Value.Replace(Value.GetBetween("https://", "@"), string.Join("", Between.Select(x => "*")));
                }
        }
        return new String(Value.Select(x => '*').ToArray());
    }
}

public enum RadzenTextMaskType
{
    All, Email, IP, Protocol
}
