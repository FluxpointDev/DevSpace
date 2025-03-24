using DevSpaceWeb.API;
using DevSpaceWeb.Data.API;
using DevSpaceWeb.Data.Teams;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace DevSpaceWeb.Controllers.API;

public class APIController : ControllerBase
{
    public APIClient Client = null!;
    public TeamData CurrentTeam = null!;
    public TeamMemberData CurrentOwner = null!;

    [NonAction]
    public JsonResult Ok()
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 200;
        return new JsonResult(new ResponseSuccess());
    }

    [NonAction]
    public JsonResult Ok(string? msg)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 200;
        return new JsonResult(new ResponseSuccess() { message = msg });
    }

    [NonAction]
    public ContentResult Ok(object? obj, string? msg = null)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 200;
        JObject JObject = new JObject();
        if (obj != null)
        {
            Type type = obj.GetType();
            if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
            {
                JObject.TryAdd("data", JArray.FromObject(obj));
            }
            else
            {
                JObject.TryAdd("data", JObject.FromObject(obj));
            }
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
    public JsonResult Conflict(string error = "The request failed due to a server issue.")
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 409;
        return new JsonResult(new Response(409, error));
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
        Response.StatusCode = 403;
        return new JsonResult(new Response(403, $"Client does not have {Utils.FriendlyName(flag.ToString())} permission."));
    }

    [NonAction]
    public JsonResult RankFailed()
    {
        Response.ContentType = "application/json";
        Response.StatusCode = 403;
        return new JsonResult(new Response(403, "This member has a higher rank than the client."));
    }
}