using Microsoft.AspNetCore.Mvc;
using DevSpaceWeb.Data.API;
using DevSpaceWeb.API;

namespace DevSpaceWeb.Controllers.API;

public class APIController : ControllerBase
{
    public new APIClient Client = null;

    [NonAction]
    public JsonResult Ok()
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 200;
        return new JsonResult(new Response(200));
    }

    [NonAction]
    public JsonResult Ok(object? obj = null)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 200;
        if (obj is string str)
            return new JsonResult(new Response(200, str));

        return new JsonResult(obj);
    }

    [NonAction]
    public JsonResult BadRequest(string error = "This request is invalid.")
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 400;
        return new JsonResult(new Response(400, error));
    }

    [NonAction]
    public JsonResult NotFound(string error = "The requested data was not found.")
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 404;
        return new JsonResult(new Response(404, error));
    }

    [NonAction]
    public JsonResult Unauthorized(string error = "You need to be authorized to use this API.")
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 401;
        return new JsonResult(new Response(401, error));
    }

    [NonAction]
    public JsonResult InternalServerError(string error = "Server error occurred.")
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 500;
        return new JsonResult(new Response(500, error));
    }

    [NonAction]
    public JsonResult Forbid(string error = "You do not have permission.")
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 403;
        return new JsonResult(new Response(403, error));
    }

    [NonAction]
    public JsonResult Forbidden(string error = "You do not have permission.")
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 403;
        return new JsonResult(new Response(403, error));
    }

    [NonAction]
    public JsonResult CustomStatus(int code, string msg)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = code;
        return new JsonResult(new Response(code, msg));
    }
}