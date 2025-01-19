using DevSpaceWeb.Translations;

namespace DevSpaceWeb;

public static class Lang
{
    public static TBase Base = new TBase();
    public static TErrors Errors = new TErrors();

    public static TServer Server = new TServer();
    public static TServerErrors ServerErrors = new TServerErrors();

    public static TConsole Console = new TConsole();
    public static TConsoleErrors ConsoleErrors = new TConsoleErrors();
}
