using DevSpaceWeb.API;
using DevSpaceWeb.Data.API;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace DevSpaceWeb.Controllers.API;

public class APIController : ControllerBase
{
    public APIClient Client = null!;

    [NonAction]
    public JsonResult Ok()
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 200;
        return new JsonResult(new Response(200));
    }

    [NonAction]
    public JsonResult Ok(string? msg)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 200;
        return new JsonResult(new Response(200, msg));
    }

    [NonAction]
    public ContentResult Ok(object obj, string? msg = null)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 200;
        JObject JObject = new JObject();
        Type type = obj.GetType();
        if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
        {
            JObject.TryAdd("data", JArray.FromObject(obj));
        }
        else
        {
            JObject.TryAdd("data", JObject.FromObject(obj));
        }

        JObject.TryAdd("success", true);
        JObject.TryAdd("code", 200);
        JObject.TryAdd("message", msg);
        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(JObject));
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

    [NonAction]
    public JsonResult PermissionFailed(Enum flag)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 400;
        return new JsonResult(new Response(400, $"Client does not have {flag.ToString()} permission."));
    }
}