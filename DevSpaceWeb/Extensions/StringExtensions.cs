using System.Web;

namespace DevSpaceWeb;

public static class StringExtensions
{
    public static string ToUpperSingle(this string String)
    {
        char[] array = String.ToArray();
        array[0] = String[0].ToString().ToUpper().ToCharArray()[0];
        return string.Join("", array);
    }

    public static string GetBetween(this string Content, string Start, string End)
    {
        int num = Content.IndexOf(Start, 0) + Start.Length;
        int num2 = Content.IndexOf(End, num);
        if (num == -1 || num2 == -1)
        {
            return "";
        }

        return Content.Substring(num, num2 - num);
    }

    public static string UrlEncoded(this string text)
    {
        return HttpUtility.UrlEncode(text);
    }

    public static ulong MentionToID(this string User)
    {
        if (User.Length < 16)
        {
            return 0;
        }

        int num = 0;
        int num2 = 0;
        switch (User[0])
        {
            case '(':
                {
                    num = 3;
                    if (User[User.Length - 1] == ')')
                    {
                        num2 = 2;
                    }

                    if (User[3] == '!')
                    {
                        num = 4;
                    }

                    if (ulong.TryParse(User.Substring(num, User.Length - num2 - num), out ulong result3))
                    {
                        return result3;
                    }

                    return 0;
                }
            case '<':
                {
                    num2 = 1;
                    num = 2;
                    if (User[2] == '!')
                    {
                        num = 3;
                    }

                    if (ulong.TryParse(User.Substring(num, User.Length - num2 - num), out ulong result2))
                    {
                        return result2;
                    }

                    return 0;
                }
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                {
                    if (ulong.TryParse(User, out ulong result))
                    {
                        return result;
                    }

                    return 0;
                }
            default:
                return 0;
        }
    }
}
