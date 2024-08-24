namespace DevSpaceWeb.Data;

public class DirectoryStructure
{
    public DirectoryStructure(string path)
    {
        Path = path;
        Console.WriteLine("Create: " + path);
        //if (!Directory.Exists(path))
        //    Directory.CreateDirectory(path);
    }

    public string Path;

    /// <summary>
    /// Fallback method for object serialize in case of code issues.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Path;
    }
}
public class DirectoryStructureMain : DirectoryStructure
{
    public DirectoryStructureMain(string folder) : base(folder)
    {
        Data = new DirectoryStructure(Path + "Data/");
        Cache = new DirectoryStructure(Path + "Cache/");
        Console.WriteLine("Main: " + folder);
        Public = new DirectoryStructurePublic(Path + "Public/");
    }

    /// <summary>
    /// Main application data folder with config.
    /// </summary>
    public DirectoryStructure Data;

    /// <summary>
    /// Cached data from certain features.
    /// </summary>
    public DirectoryStructure Cache;

    /// <summary>
    /// Public folder access from the web.
    /// </summary>
    public DirectoryStructurePublic Public;
}

public class DirectoryStructurePublic : DirectoryStructure
{
    public DirectoryStructurePublic(string folder) : base(folder)
    {
        Temp = new DirectoryStructurePublic(folder + "temp/");
    }

    /// <summary>
    /// Temp files that can accessed publicly.
    /// </summary>
    public DirectoryStructure Temp;
}
public class DirectoryStructureTemp : DirectoryStructure
{
    public DirectoryStructureTemp(string folder) : base(folder)
    {
        Images = new DirectoryStructurePublic(folder + "images/");
    }

    /// <summary>
    /// Images converted or used with image tools that can be accessed publicly.
    /// </summary>
    public DirectoryStructure Images;
}