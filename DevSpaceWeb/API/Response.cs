using System.ComponentModel;

namespace DevSpaceWeb.API;

public class Response
{
    public Response(int code, string? msg = null)
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

    public string message { get; set; } = "";
}
public class ResponseData<T> where T : class
{
    public bool success { get; set; } = true;

    [DefaultValue(200)]
    public int code { get; set; } = 200;

    public string message { get; set; } = "";

    public T? data { get; set; }
}
public class ResponseSuccess
{
    [DefaultValue(true)]
    public bool success { get; set; } = true;

    [DefaultValue(200)]
    public int code { get; set; } = 200;

    public string message { get; set; } = "";
}
public class ResponseUnauthorized
{
    [DefaultValue(false)]
    public bool success { get; set; }

    [DefaultValue(401)]
    public int code { get; set; } = 401;

    public string message { get; set; } = "";
}
public class ResponseForbidden
{
    [DefaultValue(false)]
    public bool success { get; set; }

    [DefaultValue(403)]
    public int code { get; set; } = 403;

    public string message { get; set; } = "";
}
public class ResponseBadRequest
{
    [DefaultValue(false)]
    public bool success { get; set; }

    [DefaultValue(400)]
    public int code { get; set; } = 400;

    public string message { get; set; } = "";
}
public class ResponseNotFound
{
    [DefaultValue(false)]
    public bool success { get; set; }

    [DefaultValue(404)]
    public int code { get; set; } = 404;

    public string message { get; set; } = "";

}