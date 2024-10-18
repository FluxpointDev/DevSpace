namespace DevSpaceWeb.Data;

public class DirectoryStructure
{
    public DirectoryStructure(string path)
    {
        Path = path;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
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
        Temp = new DirectoryStructureTemp(folder + "temp/");
        Resources = new DirectoryStructure(folder + "resources/");
    }

    /// <summary>
    /// Temp files that can accessed publicly.
    /// </summary>
    public DirectoryStructureTemp Temp;

    public DirectoryStructure Resources;
}
public class DirectoryStructureTemp : DirectoryStructure
{
    public DirectoryStructureTemp(string folder) : base(folder)
    {
        Images = new DirectoryStructure(folder + "images/");
    }

    /// <summary>
    /// Images converted or used with image tools that can be accessed publicly.
    /// </summary>
    public DirectoryStructure Images;
}