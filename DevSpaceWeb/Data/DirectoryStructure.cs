namespace DevSpaceWeb.Data;

public class DirectoryStructure
{
    public DirectoryStructure(string path)
    {
        Path = path;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        Data = new DirectoryStructure(Path + "Data/");
        Cache = new DirectoryStructure(Path + "Cache/");
        Public = new DirectoryStructurePublic(Path + "Public/");
    }

    public string Path;

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
    public DirectoryStructure Public;

    /// <summary>
    /// Fallback method for object serialize in case of code issues.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Path;
    }
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