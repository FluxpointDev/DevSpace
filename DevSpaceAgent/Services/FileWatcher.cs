using DevSpaceAgent.Data;
using Newtonsoft.Json;

namespace DevSpaceAgent.Services;

public static class FileWatcher
{
    public static FileSystemWatcher ConfigWatch;

    public static void Start()
    {
        ConfigWatch = new FileSystemWatcher
        {
            Path = Program.CurrentDirectory = "Data",
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.LastWrite,
            Filter = "*Config.json"
        };
        ConfigWatch.Changed += ConfigWatch_Changed;
    }

    private static DateTime lastRead = DateTime.MinValue;

    private static void ConfigWatch_Changed(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
            return;

        DateTime lastWriteTime = File.GetLastWriteTime(Program.CurrentDirectory + "Data/Config.json");
        if (lastWriteTime == lastRead)
        {

            Config? Config = null;
            try
            {
                using (StreamReader reader = new StreamReader(Program.CurrentDirectory + "Data/Config.json", new FileStreamOptions { Access = FileAccess.Read, Mode = FileMode.Open, Share = FileShare.Read }))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    Config = (Config)serializer.Deserialize(reader, typeof(Config));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to parse Config.json file, " + ex.Message);
                return;
            }
            if (Config == null)
            {
                Console.WriteLine("Failed to load Config.json file.");
                return;
            }

            _Data.LoadConfig();
        }

        lastRead = lastWriteTime;
    }
}
