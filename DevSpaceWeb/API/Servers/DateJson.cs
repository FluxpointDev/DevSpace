namespace DevSpaceWeb.API.Servers;

public class DateJson
{
    public DateJson(string date)
    {
        utc = DateTimeOffset.Parse(date);
        unix = utc.ToUnixTimeSeconds();
    }
    public DateJson(DateTime date)
    {
        utc = (DateTimeOffset)date;
        unix = utc.ToUnixTimeSeconds();
    }

    public DateTimeOffset utc { get; set; }
    public long unix { get; set; }
}
