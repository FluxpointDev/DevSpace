using DevSpaceWeb.Data;
using Fido2NetLib;
using System.Text;

namespace DevSpaceWeb.Fido2;

public class Fido2Service
{
    public Fido2Service()
    {
        SetConfig();
    }

    public void SetConfig()
    {
        HashSet<string> Origins = new HashSet<string>();
        if (Program.IsDevMode)
        {
            Origins.Add("https://localhost:5149");
        }
        else
        {
            Origins.Add("https://" + _Data.Config.Instance.PublicDomain);
        }

        _lib = new Fido2NetLib.Fido2(new Fido2Configuration
        {
            ServerDomain = Program.IsDevMode ? "localhost" : _Data.Config.Instance.PublicDomain,
            ServerName = _Data.Config.Instance.Name,
            Origins = Origins,
            TimestampDriftTolerance = 300000,
            MDSCacheDirPath = Program.Directory.Cache.Path
        });
    }

    public Fido2NetLib.Fido2 _lib { get; private set; }
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