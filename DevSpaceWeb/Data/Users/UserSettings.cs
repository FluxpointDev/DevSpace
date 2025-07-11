namespace DevSpaceWeb.Data.Users;

public class UserSettings
{
    public DateFormatLang DateFormat { get; set; } = DateFormatLang.Automatic;
    public bool Use24HourTime { get; set; }
}
