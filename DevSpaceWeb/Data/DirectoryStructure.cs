namespace DevSpaceWeb.Data;

public class DirectoryStructure
{
    public DirectoryStructure(string path)
    {
        Path = path;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        Data = new DirectoryStructure(Path + "Data/");
        Public = new DirectoryStructurePublic(Path + "Public/");
    }

    public string Path;

    public DirectoryStructure Data;
    public DirectoryStructure Public;
}
public class DirectoryStructurePublic : DirectoryStructure
{
    public DirectoryStructurePublic(string folder) : base(folder)
    {
        Temp = new DirectoryStructurePublic(folder + "Temp/");
    }

    public DirectoryStructure Temp;
}