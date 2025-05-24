namespace DevSpaceShared;
public static class StringExtensions
{
    public static string[] SplitNewlines(this string? input, bool trim = false)
    {
        if (string.IsNullOrEmpty(input))
            return [];

        return input.Split(["\r\n", "\r", "\n"], trim ? StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries : StringSplitOptions.RemoveEmptyEntries);
    }

    public static double RoundNearestBytes(this double number)
    {
        // Bytes
        if (number < 1024.0)
            return number;

        // Round to kilobytes
        double Round = number / 1024.0;

        // Return kilobytes
        if (Round < 1024.0)
            return Double.Round(Round, 0, MidpointRounding.ToEven) * 1024;

        // Round to megabytes
        Round = Round / 1024.0;

        // Return megabytes
        if (Round < 1024.0)
            return (Double.Round(Round, 0, MidpointRounding.ToEven) * 1024) * 1024;

        // Round to gigabytes
        Round = Round / 1024.0;

        // Return gigabytes
        if (Round < 1024.0)
            return (Double.Round(Round, 2, MidpointRounding.ToEven) * 1024) * 1024 * 1024;

        // Round to petabytes
        Round = Round / 1024.0;

        return (Double.Round(Round, 2, MidpointRounding.ToEven) * 1024) * 1024 * 1024 * 1024;
    }
}
