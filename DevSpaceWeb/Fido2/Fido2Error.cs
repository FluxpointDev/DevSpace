using Fido2NetLib;

namespace DevSpaceWeb.Fido2;

public class Fido2Error : Fido2ResponseBase
{
    public Fido2Error(string message) : base()
    {
        Status = "error";
        ErrorMessage = message;
    }
}
