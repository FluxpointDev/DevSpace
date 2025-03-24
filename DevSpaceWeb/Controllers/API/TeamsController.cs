using DevSpaceWeb.API;
using DevSpaceWeb.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class TeamsController : APIController
{
    //[HttpGet("/api/team")]
    //[SwaggerOperation("Get a team.", "")]
    //[SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<TeamJson>))]
    //[SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    //public async Task<IActionResult> GetTeam()
    //{
    //    return Ok(new TeamJson(CurrentTeam, Client.HasTeamPermission(CurrentTeam, TeamPermission.ViewPermissions)));
    //}




}
