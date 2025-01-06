namespace DevSpaceWeb.API;

public class Response
{
    public Response(int code, string msg = "")
    {
        this.code = code;
        message = msg;
        switch (code)
        {
            case 200:
            case 418:
                success = true;
                break;
        }
    }

    public bool success { get; set; }
    public int code { get; set; }
    public string message { get; set; }
}
public class ResponseUnauthorized
{
    public bool success { get; set; }
    public int code { get; set; } = 401;
    public string message { get; set; }
}
public class ResponseForbidden
{
    public bool success { get; set; }
    public int code { get; set; } = 403;
    public string message { get; set; }
}
public class ResponseBadRequest
{
    public bool success { get; set; }
    public int code { get; set; } = 400;
    public string message { get; set; }
}
public class ResponseNotFound
{
    public bool success { get; set; }
    public int code { get; set; } = 404;
    public string message { get; set; }
}