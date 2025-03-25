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

    public bool success;
    public int code;

    public string message = "";
}
public class ResponseData<T> where T : class
{
    public bool success = true;

    [DefaultValue(200)]
    public int code = 200;

    public string message = "";

    public T? data;
}
public class ResponseSuccess
{
    [DefaultValue(true)]
    public bool success = true;

    [DefaultValue(200)]
    public int code = 200;

    public string message = "";
}
public class ResponseUnauthorized
{
    [DefaultValue(false)]
    public bool success;

    [DefaultValue(401)]
    public int code = 401;

    public string message = "";
}
public class ResponseForbidden
{
    [DefaultValue(false)]
    public bool success;

    [DefaultValue(403)]
    public int code = 403;

    public string message = "";
}
public class ResponseBadRequest
{
    [DefaultValue(false)]
    public bool success;

    [DefaultValue(400)]
    public int code = 400;

    public string message = "";
}
public class ResponseNotFound
{
    [DefaultValue(false)]
    public bool success;

    [DefaultValue(404)]
    public int code = 404;

    public string message = "";

}