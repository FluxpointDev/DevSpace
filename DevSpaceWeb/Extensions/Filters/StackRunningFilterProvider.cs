namespace DevSpaceWeb.Extensions.Filters;

public class StackRunningFilterProvider : IFormatProvider, ICustomFormatter
{
    public static StackRunningFilterProvider Instance = new StackRunningFilterProvider();

    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
        return object.Equals(arg, true) ? "Running" : "Down";
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
