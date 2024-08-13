using Fido2NetLib;
using System.Text;

namespace DevSpaceWeb.Fido2;

public class Fido2Service
{
    public Fido2Service(Fido2Configuration config)
    {
        //var repos = new List<IMetadataRepository>
        //{
        //    new ConformanceMetadataRepository(Program.Http, config.Origins.First()),
        //    new FileSystemMetadataRepository(config.MDSCacheDirPath)
        //};
        //_metadata = new Fido2Metadata(repos);
        _lib = new Fido2NetLib.Fido2(config);
        //_metadata.InitializeAsync();
    }

    public readonly Fido2NetLib.Fido2 _lib;
    public readonly Fido2Metadata _metadata;

    public static byte[] GetPasskeyIdInBytes(string? passykeyId)
    {
        if (passykeyId != null)
        {
            return Encoding.UTF8.GetBytes(passykeyId);
        }

        throw new ArgumentNullException(nameof(passykeyId));
    }
}