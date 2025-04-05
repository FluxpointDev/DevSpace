namespace DevSpaceWeb.Data;

public class SessionTask
{
    public string? Icon { get; set; }
    public string? Text { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonLink { get; set; }
    public Action? ButtonTask { get; set; }
    public bool? IsSuccess { get; set; }
}
