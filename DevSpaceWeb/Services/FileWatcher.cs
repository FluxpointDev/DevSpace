using DevSpaceWeb.Data;
using Newtonsoft.Json;

namespace DevSpaceWeb.Services;

public static class FileWatcher
{
    public static FileSystemWatcher ConfigWatch;
    public static void Start()
    {
        Console.WriteLine("Start file watch: " + Program.Directory.Data.Path);
        ConfigWatch = new FileSystemWatcher
        {
            Path = Program.Directory.Data.Path,
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.LastWrite,
            Filter = "Config.json"
        };
        ConfigWatch.Changed += ConfigWatch_Changed;
    }

    private static DateTime lastRead = DateTime.MinValue;

    private static void ConfigWatch_Changed(object sender, FileSystemEventArgs e)
    {

        if (e.ChangeType != WatcherChangeTypes.Changed)
            return;

        DateTime lastWriteTime = File.GetLastWriteTime(Program.Directory.Data.Path + "Data/Config.json");
        if (lastWriteTime == lastRead)
        {
            Console.WriteLine($"Change: {e.Name} {e.ChangeType}");
            Task.Delay(1000);
            Config? Config = null;
            try
            {
                using (StreamReader reader = new StreamReader(Program.Directory.Data.Path + "Config.json", new FileStreamOptions { Access = FileAccess.Read, Mode = FileMode.Open, Share = FileShare.Read }))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    Config = (Config)serializer.Deserialize(reader, typeof(Config));
                }

                if (Config == null)
                {
                    Console.WriteLine("Failed to load Config.json file.");
                }
                else
                {
                    if (_Data.LoadConfig())
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to parse Config.json file, " + ex.Message);
            }
        }

        lastRead = lastWriteTime;
    }
}
