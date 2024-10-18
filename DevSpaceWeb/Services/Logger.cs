using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace DevSpaceWeb;

/// <summary>
/// Special logger class with custom title and console colors.
/// </summary>
public static class Logger
{
    /// <summary>
    /// Initialize your own logging system with a custom title and log mode.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="logMode"></param>
    public static void RunLogger(string title, LogSeverity logMode)
    {
        Title = title;
        LogMode = logMode;
        LoggerTask = Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                foreach (LogJsonMessage msg in MessageQueue.GetConsumingEnumerable())
                {
                    string title = string.IsNullOrEmpty(msg.Title) ? Title : msg.Title;
                    if (msg.Data == null)
                    {
                        Console.WriteLine($"[{title}] {msg.Color}{msg.Message}{Reset}");
                    }
                    else if (msg.Data is string str)
                    {
                        Console.WriteLine($"[{title}] {msg.Color}{msg.Message}\n" +
                            $"--- --- ---\n" +
                            $"{FormatJsonPretty(str)}\n" +
                            $"--- --- ---{Reset}");
                    }
                    else
                    {
                        Console.WriteLine($"[{title}] {msg.Color}{msg.Message}\n" +
                            $"--- --- ---\n" +
                            $"{FormatJsonPretty(msg.Data)}\n" +
                            $"--- --- ---{Reset}");
                    }

                }
            }
            

        }, TaskCreationOptions.LongRunning);
    }

    private static string Title { get; set; }

    private static Task LoggerTask { get; set; }

    private static LogSeverity LogMode { get; set; }

    private static BlockingCollection<LogJsonMessage> MessageQueue = new BlockingCollection<LogJsonMessage>();

#pragma warning disable IDE0051 // Remove unused private members

    /// <summary> Reset console color </summary>
    public static readonly string Reset = "\u001b[39m";
    /// <summary> Red console color </summary>
    public static readonly string Red = "\u001b[31m";
    /// <summary> Light Red console color </summary>
    public static readonly string LightRed = "\u001b[91m";
    /// <summary> Green console color </summary>
    public static readonly string Green = "\u001b[32m";
    /// <summary> Light Green console color </summary>
    public static readonly string LightGreen = "\u001b[92m";
    /// <summary> Yellow console color </summary>
    public static readonly string Yellow = "\u001b[33m";
    /// <summary> Light Yellow console color </summary>
    public static readonly string LightYellow = "\u001b[93m";
    /// <summary> Blue console color </summary>
    public static readonly string Blue = "\u001b[34m";
    /// <summary> Light Blue console color </summary>
    public static readonly string LightBlue = "\u001b[94m";
    /// <summary> Magenta console color </summary>
    public static readonly string Magenta = "\u001b[35m";
    /// <summary> Light Magenta console color </summary>
    public static readonly string LightMagenta = "\u001b[95m";
    /// <summary> Cyan console color </summary>
    public static readonly string Cyan = "\u001b[36m";
    /// <summary> Light Cyan console color </summary>
    public static readonly string LightCyan = "\u001b[96m";
    /// <summary> Grey console color </summary>
    public static readonly string Grey = "\u001b[90m";
    /// <summary> Light Grey console color </summary>
    public static readonly string LightGrey = "\u001b[37m";

#pragma warning restore IDE0051 // Remove unused private members


    private static string FormatJsonPretty(string json)
    {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        

    }

    private static string FormatJsonPretty(object json)
    {
        return JsonConvert.SerializeObject(json, Formatting.Indented);
    }

    /// <summary>
    /// Special json log message with json data that can be a json string or class/object
    /// </summary>
    public static void LogJson(string message, object data)
    {
        MessageQueue.Add(new LogJsonMessage { Message = message, Data = data, Color = LightMagenta });
    }


    public static void LogMessage(string message, LogSeverity severity)
        => LogMessage(Title, message, severity);

    /// <summary>
    /// Log a message to the console with the severity color
    /// <para>Info: White</para>
    /// <para>Warn: Yellow</para>
    /// <para>Error: Red</para>
    /// <para>Debug: Grey</para>
    /// </summary>
    public static void LogMessage(string title, string message, LogSeverity severity)
    {
        if (severity < LogMode)
            return;

        switch (severity)
        {
            // White
            case LogSeverity.Info:
                {
                    MessageQueue.Add(new LogJsonMessage { Title = title, Message = message, Color = LightCyan });
                }
                break;
            // Yellow
            case LogSeverity.Warn:
                {
                    MessageQueue.Add(new LogJsonMessage { Title = title, Message = message, Color = Yellow });
                }
                break;
            // Red
            case LogSeverity.Error:
                {
                    MessageQueue.Add(new LogJsonMessage { Title = title, Message = message, Color = Red });
                }
                break;
            // Grey
            case LogSeverity.Debug:
                {
                    MessageQueue.Add(new LogJsonMessage { Title = title, Message = message, Color = Grey });
                }
                break;
        }
    }

    /// <summary>
    /// Log a rest response to the console with the color
    /// <para>Success: Green</para>
    /// <para>Fail: Light Red</para>
    /// </summary>
    public static void LogRestMessage(HttpResponseMessage res, HttpMethod method, string message)
    {
        if (res.IsSuccessStatusCode)
        {
            MessageQueue.Add(new LogJsonMessage { Title = "Request", Message = $"({method.Method.ToUpper()}) {message}", Color = Green });
        }
        else
        {
            MessageQueue.Add(new LogJsonMessage { Title = "Rest", Message = $"({method.Method.ToUpper()}) {message}", Color = LightRed });
        }
    }
}

internal class LogJsonMessage
{
    public string? Title;
    public string Message;
    public object Data;
    public string Color = "";
}

/// <summary>
/// The severity of a log message raised by <see cref="RevoltClient.OnLog"/>.
/// </summary>
public enum LogSeverity
{
    /// <summary>
    /// All messages including debug ones.
    /// </summary>
    Debug,

    /// <summary>
    /// Error message info.
    /// </summary>
    Error,

    /// <summary>
    /// Log info and warning messages.
    /// </summary>
    Warn,

    /// <summary>
    /// Only log info messages.
    /// </summary>
    Info,

    /// <summary>
    /// Do not log anything.
    /// </summary>
    None
}
