namespace DevSpaceShared;
public static class StringExtensions
{
    public static string[] SplitNewlines(this string? input, bool trim = false)
    {
        if (string.IsNullOrEmpty(input))
            return Array.Empty<string>();

        return input.Split(["\r\n", "\r", "\n"], trim ? StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries : StringSplitOptions.RemoveEmptyEntries);
    }
}
