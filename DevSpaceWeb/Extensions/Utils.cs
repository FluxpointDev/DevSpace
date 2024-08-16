using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace DevSpaceWeb;

public static class Utils
{
    public static CultureInfo GetCultureFromTwoLetterCountryCode(string twoLetterISOCountryCode)
    {
        try
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures
                                               & ~CultureTypes.NeutralCultures)
                      .Where(m => m.Name.EndsWith("-" + twoLetterISOCountryCode))
                      .FirstOrDefault();
        }
        catch
        {
            return new CultureInfo("GB");
        }
    }

    public static string GetUserIpAddress(HttpContext context)
    {
        // Test ip in development
        if (Program.IsDevMode)
            return "1.2.3.4";

        // Check CF-Connecting-IP header
        if (!string.IsNullOrEmpty(context.Request.Headers["CF-CONNECTING-IP"]))
            return context.Request.Headers["CF-CONNECTING-IP"];

        // Check X-Forwarded-For header
        if (!string.IsNullOrEmpty(context.Request.Headers["X-Forwarded-For"]))
            return context.Request.Headers["X-Forwarded-For"];

        return "";
    }

    internal static string GetStringSha256Hash(string text)
    {
        if (String.IsNullOrEmpty(text))
            return String.Empty;

        using (var sha = new System.Security.Cryptography.SHA256Managed())
        {
            byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = sha.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }
    }


    /// <summary>
    /// Generate a new recovery code.
    /// </summary>
    /// <returns></returns>
    public static string CreateTwoFactorRecoveryCode()
    {
        return string.Create(24, 0, static (buffer, _) =>
        {
            buffer[23] = GetRandomRecoveryCodeChar();
            buffer[22] = GetRandomRecoveryCodeChar();
            buffer[21] = GetRandomRecoveryCodeChar();
            buffer[20] = GetRandomRecoveryCodeChar();
            buffer[19] = '-';
            buffer[18] = GetRandomRecoveryCodeChar();
            buffer[17] = GetRandomRecoveryCodeChar();
            buffer[16] = GetRandomRecoveryCodeChar();
            buffer[15] = GetRandomRecoveryCodeChar();
            buffer[14] = '-';
            buffer[13] = GetRandomRecoveryCodeChar();
            buffer[12] = GetRandomRecoveryCodeChar();
            buffer[11] = GetRandomRecoveryCodeChar();
            buffer[10] = GetRandomRecoveryCodeChar();
            buffer[9] = '-';
            buffer[8] = GetRandomRecoveryCodeChar();
            buffer[7] = GetRandomRecoveryCodeChar();
            buffer[6] = GetRandomRecoveryCodeChar();
            buffer[5] = GetRandomRecoveryCodeChar();
            buffer[4] = '-';
            buffer[3] = GetRandomRecoveryCodeChar();
            buffer[2] = GetRandomRecoveryCodeChar();
            buffer[1] = GetRandomRecoveryCodeChar();
            buffer[0] = GetRandomRecoveryCodeChar();
        });
    }

    private static readonly char[] AllowedChars = "23456789BCDFGHJKMNPQRTVWXY".ToCharArray();
    private static char GetRandomRecoveryCodeChar()
    {
        // Based on RandomNumberGenerator implementation of GetInt32
        uint range = (uint)AllowedChars.Length - 1;

        // Create a mask for the bits that we care about for the range. The other bits will be
        // masked away.
        uint mask = range;
        mask |= mask >> 1;
        mask |= mask >> 2;
        mask |= mask >> 4;
        mask |= mask >> 8;
        mask |= mask >> 16;

        Span<uint> resultBuffer = stackalloc uint[1];

        uint result;

        do
        {
            RandomNumberGenerator.Fill(MemoryMarshal.AsBytes(resultBuffer));

            result = mask & resultBuffer[0];
        }
        while (result > range);

        return AllowedChars[(int)result];
    }
}
