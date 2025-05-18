namespace DevSpaceWeb.Fido2;

public class Fido2Error
{
    public Fido2Error(string message)
    {
        ErrorMessage = message;
    }

    public string Status { get; set; } = "error";
    public string ErrorMessage { get; set; }
}
