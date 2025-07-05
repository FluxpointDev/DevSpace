namespace DevSpaceWeb;

public class EnabledFilterProvider : IFormatProvider, ICustomFormatter
{
    public static EnabledFilterProvider Instance = new EnabledFilterProvider();

    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
        return object.Equals(arg, true) ? "Enabled" : "Disabled";
    }

    public object GetFormat(Type formatType)
    {
        if (formatType == typeof(ICustomFormatter))
        {
            return this;
        }

        return null;
    }
}
public class DisabledFilterProvider : IFormatProvider, ICustomFormatter
{
    public static DisabledFilterProvider Instance = new DisabledFilterProvider();

    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
        return object.Equals(arg, false) ? "Enabled" : "Disabled";
    }

    public object GetFormat(Type formatType)
    {
        if (formatType == typeof(ICustomFormatter))
        {
            return this;
        }

        return null;
    }
}