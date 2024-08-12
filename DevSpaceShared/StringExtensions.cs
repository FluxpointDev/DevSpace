namespace DevSpaceShared;
public static class StringExtensions
{
    public static string[] SplitNewlines(this string input)
    {
        return input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
    }
}
