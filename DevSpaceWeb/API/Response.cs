﻿namespace DevSpaceWeb.API;

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
public class ResponseUnauthorized : Response
{
    public ResponseUnauthorized() : base(401, "")
    {

    }
}
public class ResponseForbidden : Response
{
    public ResponseForbidden() : base(403, "")
    {

    }
}
public class ResponseBadRequest : Response
{
    public ResponseBadRequest() : base(403, "")
    {

    }
}