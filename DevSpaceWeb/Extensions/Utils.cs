using DevSpaceWeb.Components.Layout;
using DevSpaceWeb.Data.Users;
using Microsoft.AspNetCore.Identity;
using Radzen;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace DevSpaceWeb;

public static class Utils
{
    public static bool IsDockerImageNameValid(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        if (input.Length < 2)
            return false;

        if (input.Length > 255)
            return false;

        foreach (char i in input)
        {
            if (!(char.IsLower(i) ||
            char.IsDigit(i) ||
            i == ':' ||
            i == '/' ||
            i == '.' ||
            i == '_' ||
            i == '-'))
                return false;
        }

        return true;

    }

    public static string? FormatVanityUrl(string? vanityUrl)
    {
        if (string.IsNullOrEmpty(vanityUrl))
            return null;

        return vanityUrl.Replace(" ", "-").ToLower();
    }

    public static string? GetDomain(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return url;

        return url.Replace("https://", "").Replace("http://", "").Split('/').First();
    }
    public static string GetBrowserName(SessionBrowserType type)
    {
        return type switch
        {
            SessionBrowserType.Chrome => "Chrome",
            SessionBrowserType.Edge => "Edge",
            SessionBrowserType.Firefox => "Firefox",
            SessionBrowserType.InternetExplorer => "Internet Explorer",
            SessionBrowserType.Opera => "Opera",
            SessionBrowserType.Safari => "Safari",
            SessionBrowserType.Vivaldi => "Vivaldi",
            SessionBrowserType.Unknown => "Unknown",
            _ => "Unknown"
        };
    }
    public static UserPasswordStrength GetPasswordStrength(string password)
    {
        int Capitals = 0;
        int Lower = 0;
        int Digits = 0;
        int Special = 0;

        foreach (char c in password)
        {
            if (char.IsUpper(c))
                Capitals += 1;
            else if (char.IsLower(c))
                Lower += 1;
            else if (char.IsDigit(c))
                Digits += 1;
            else if (char.IsSymbol(c))
                Special += 1;
        }

        if (Capitals == 0 && Digits == 0)
            return UserPasswordStrength.Low;

        if (Capitals == 0 && Special == 0)
            return UserPasswordStrength.Low;

        if (Digits == 0 && Special == 0)
            return UserPasswordStrength.Low;

        if (password.Contains("123"))
            return UserPasswordStrength.Low;

        if (Capitals != 0 && Lower != 0)
        {
            if (Capitals > 1 && Digits > 2 && password.Length > 16)
                return UserPasswordStrength.High;

            if (Capitals > 1 && Special > 2 && password.Length > 16)
                return UserPasswordStrength.High;

            if (Digits > 2 && Special > 2 && password.Length > 16)
                return UserPasswordStrength.High;
        }

        return UserPasswordStrength.Normal;
    }

    private static readonly string[] SizeSuffixes =
                   ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
    public static string SizeSuffix(Int64 value, int decimalPlaces = 0)
    {
        if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
        if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
        if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} B", 0); }

        // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
        int mag = (int)Math.Log(value, 1024);

        // 1L << (mag * 10) == 2 ^ (10 * mag) 
        // [i.e. the number of bytes in the unit corresponding to mag]
        decimal adjustedSize = (decimal)value / (1L << (mag * 10));

        // make adjustment when the value is large enough that
        // it would round up to 1000 or more
        if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
        {
            mag += 1;
            adjustedSize /= 1024;
        }

        if (mag > 2)
            decimalPlaces = 2;

        return string.Format("{0:n" + decimalPlaces + "} {1}",
            adjustedSize,
            SizeSuffixes[mag]);
    }

    private const long OneKb = 1024;
    private const long OneMb = OneKb * 1024;
    private const long OneGb = OneMb * 1024;
    private const long OneTb = OneGb * 1024;

    public static string ToPrettySize(long value, int decimalPlaces = 0)
    {
        double asTb = Math.Round((double)value / OneTb, decimalPlaces);
        double asGb = Math.Round((double)value / OneGb, decimalPlaces);
        double asMb = Math.Round((double)value / OneMb, decimalPlaces);
        double asKb = Math.Round((double)value / OneKb, decimalPlaces);
        string chosenValue = asTb > 1 ? string.Format("{0} TB", asTb)
            : asGb > 1 ? string.Format("{0} GB", asGb)
            : asMb > 1 ? string.Format("{0} MB", asMb)
            : asKb > 1 ? string.Format("{0} KB", asKb)
            : string.Format("{0} B", Math.Round((double)value, decimalPlaces));
        return chosenValue;
    }

    public static string ToPrettySize(ulong value, int decimalPlaces = 0)
    {
        double asTb = Math.Round((double)value / OneTb, decimalPlaces);
        double asGb = Math.Round((double)value / OneGb, decimalPlaces);
        double asMb = Math.Round((double)value / OneMb, decimalPlaces);
        double asKb = Math.Round((double)value / OneKb, decimalPlaces);
        string chosenValue = asTb > 1 ? string.Format("{0} TB", asTb)
            : asGb > 1 ? string.Format("{0} GB", asGb)
            : asMb > 1 ? string.Format("{0} MB", asMb)
            : asKb > 1 ? string.Format("{0} KB", asKb)
            : string.Format("{0} B", Math.Round((double)value, decimalPlaces));
        return chosenValue;
    }

    public static CultureInfo? GetCultureFromTwoLetterCountryCode(string twoLetterISOCountryCode)
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

    public static string? GetUserIpAddress(HttpContext context)
    {
        // Test ip in development
        if (Program.IsDevMode || Program.IsPreviewMode)
            return "1.2.3.4";

        if (!string.IsNullOrEmpty(context.Request.Headers["CF-Connecting-IPv6"]))
            return context.Request.Headers["CF-Connecting-IPv6"];

        // Check CF-Connecting-IP header
        if (!string.IsNullOrEmpty(context.Request.Headers["CF-Connecting-IP"]))
            return context.Request.Headers["CF-Connecting-IP"];

        // Check X-Forwarded-For header
        if (!string.IsNullOrEmpty(context.Request.Headers["X-Forwarded-For"]))
            return context.Request.Headers["X-Forwarded-For"];

        return context.Connection.RemoteIpAddress?.ToString();
    }

    internal static string GetStringSha256Hash(string text)
    {
        if (String.IsNullOrEmpty(text))
            return String.Empty;

#pragma warning disable SYSLIB0021 // Type or member is obsolete
        using (SHA256Managed sha = new SHA256Managed())
        {
            byte[] textData = Encoding.UTF8.GetBytes(text);
            byte[] hash = sha.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }
#pragma warning restore SYSLIB0021 // Type or member is obsolete
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

    public static string FriendlyName(string text, bool preserveAcronyms = true)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        if (text.Equals("apis", StringComparison.OrdinalIgnoreCase))
            return text;

        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
                if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                    (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                     i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString().Replace("2 FA", " 2FA");
    }

    private static readonly char[] AllowedChars = "23456789BCDFGHJKMNPQRTVWXY_-.<>+=!?$*[]()%".ToCharArray();
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

    public static string GetLocalDate(SessionProvider session, DateTime? date, bool isMini = true, bool showTime = false, bool showSeconds = false)
    {
        if (!date.HasValue)
            return "";

        if (date.Value.Year == 0001 || date.Value.Year == 1601)
            return "";

        DateTimeOffset MessageOffset = date.Value.AddMinutes(session.UserDateOffset);
        DateTimeOffset UserDate = DateTimeOffset.UtcNow.AddMinutes(session.UserDateOffset);

        if (showSeconds)
            return MessageOffset.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

        if (isMini && MessageOffset.Year == UserDate.Year && MessageOffset.Month == UserDate.Month)
        {
            if (MessageOffset.Day == UserDate.Day)
                return $"Today at {MessageOffset.ToString("hh:mm tt", CultureInfo.InvariantCulture)}";

            DateTimeOffset Lastday = UserDate.AddDays((int)-1);
            if (MessageOffset.Day == Lastday.Day)
                return $"Yesterday at {MessageOffset.ToString("hh:mm tt", CultureInfo.InvariantCulture)}";
        }

        switch (session.UserDateFormat)
        {
            case DateFormatLang.DMY_Dash:
                return MessageOffset.ToString("dd-MM-yyyy") + (showTime ? " " + MessageOffset.ToString("hh:mm tt", CultureInfo.InvariantCulture) : "");
            case DateFormatLang.DMY_Dot:
                return MessageOffset.ToString("dd.MM.yyyy") + (showTime ? " " + MessageOffset.ToString("hh:mm tt", CultureInfo.InvariantCulture) : "");
            case DateFormatLang.DMY_Slash:
                return MessageOffset.ToString("dd/MM/yyyy") + (showTime ? " " + MessageOffset.ToString("hh:mm tt", CultureInfo.InvariantCulture) : "");
            case DateFormatLang.MDY_Dash:
                return MessageOffset.ToString("MM-dd-yyyy") + (showTime ? " " + MessageOffset.ToString("hh:mm tt", CultureInfo.InvariantCulture) : "");
            case DateFormatLang.MDY_Dot:
                return MessageOffset.ToString("MM.dd.yyyy") + (showTime ? " " + MessageOffset.ToString("hh:mm tt", CultureInfo.InvariantCulture) : "");
            case DateFormatLang.MDY_Slash:
                return MessageOffset.ToString("MM/dd/yyyy") + (showTime ? " " + MessageOffset.ToString("hh:mm tt", CultureInfo.InvariantCulture) : "");
            case DateFormatLang.YMD_Dash:
                return MessageOffset.ToString("yyyy-MM-dd") + (showTime ? " " + MessageOffset.ToString("hh:mm tt", CultureInfo.InvariantCulture) : "");
            case DateFormatLang.YMD_Dot:
                return MessageOffset.ToString("yyyy.MM.dd") + (showTime ? " " + MessageOffset.ToString("hh:mm tt", CultureInfo.InvariantCulture) : "");
            case DateFormatLang.YMD_Slash:
                return MessageOffset.ToString("yyyy/MM/dd") + (showTime ? " " + MessageOffset.ToString("hh:mm tt", CultureInfo.InvariantCulture) : "");
        }
        return MessageOffset.ToString("dd/MM/yyyy") + (showTime ? " " + MessageOffset.ToString("hh:mm tt", CultureInfo.InvariantCulture) : "");
    }

    public static DateFormatLang GetDateFormat(string lang)
    {
        switch (lang)
        {
            case "af":
            case "fr-ca":
            case "ku-arab":
            case "lt":
            case "mi-latn":
            case "rw":
            case "sd-arab":
            case "si":
            case "sv":
            case "tn":
            case "ug-arab":
            case "zu":
            case "hu":
            case "ko":
            case "sk":
            case "sl":
                return DateFormatLang.YMD_Dash;

            case "am":
            case "ar-sa":
            case "en-us":
            case "es-us":
            case "fil-latn":
            case "or":
                return DateFormatLang.MDY_Slash;

            case "as":
            case "kok":
            case "nl":
            case "nl-be":
            case "te":
            case "wo":
            case "cs":
                return DateFormatLang.DMY_Dash;

            case "az-latn":
            case "be":
            case "bg":
            case "bs":

            case "da":
            case "de":
            case "de-de":
            case "et":
            case "fi":
            case "he":
            case "hr":
            case "hy":
            case "is":
            case "ka":
            case "kk":
            case "lb":
            case "lv":
            case "mk":
            case "nb":
            case "nn":
            case "pl":
            case "ro":
            case "ru":
            case "sq":
            case "sr-cyrl-ba":
            case "sr-cyrl-rs":
            case "sr-latn-rs":
            case "tk-latn":
            case "tr":
            case "tt-cyrl":
            case "uk":
                return DateFormatLang.DMY_Dot;

            case "bn-bd":
            case "bn-in":
            case "ca":
            case "ca-es-valencia":
            case "cy":
            case "el":
            case "en-gb":
            case "es":
            case "es-es":
            case "es-mx":
            case "fr":
            case "fr-fr":
            case "ga":
            case "gd-latn":
            case "gl":
            case "gu":
            case "ha-latn":
            case "hi":
            case "id":
            case "ig-latn":
            case "it":
            case "it-it":
            case "km":
            case "kn":
            case "ky-cyrl":
            case "ml":
            case "mr":
            case "ms":
            case "mt":
            case "nso":
            case "pa":
            case "pa-arab":
            case "pt-br":
            case "pt-pt":
            case "qut-latn":
            case "quz":
            case "sw":
            case "ta":
            case "tg-cyrl":
            case "th":
            case "ti":
            case "ur":
            case "uz-latn":
            case "vi":
            case "yo-latn":
                return DateFormatLang.DMY_Slash;

            case "eu":
            case "fa":
            case "ja":
            case "ne":
            case "prs-arab":
            case "xh":
            case "zh-hans":
            case "zh-hant":
                return DateFormatLang.YMD_Slash;

            case "mn-cyrl":
                return DateFormatLang.YMD_Dot;
        }
        return DateFormatLang.DMY_Slash;
    }

    public static PasswordHasher<AuthUser> Hasher = new PasswordHasher<AuthUser>();
}
public enum DateFormatLang
{
    Automatic,
    [Display(Description = "Day/Month/Year ( 31/12/2024 )")]
    DMY_Slash,
    [Display(Description = "Month/Day/Year ( 12/31/2024 )")]
    MDY_Slash,
    [Display(Description = "Year/Month/Day ( 2024/12/31 )")]
    YMD_Slash,
    [Display(Description = "Day-Month-Year ( 31-12-2024 )")]
    DMY_Dash,
    [Display(Description = "Month-Day-Year ( 12-31-2024 )")]
    MDY_Dash,
    [Display(Description = "Year-Month-Day ( 2024-12-31 )")]
    YMD_Dash,
    [Display(Description = "Day.Month.Year ( 31.12.2024 )")]
    DMY_Dot,
    [Display(Description = "Month.Day.Year ( 12.31.2024 )")]
    MDY_Dot,
    [Display(Description = "Year.Month.Day ( 2024.12.31 )")]
    YMD_Dot
}
