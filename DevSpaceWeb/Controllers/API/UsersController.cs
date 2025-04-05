using DevSpaceWeb.API;
using DevSpaceWeb.API.Users;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[SwaggerTag("Requires permission View Members")]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class UsersController : APIController
{
    [HttpGet("/api/users/{userId?}")]
    [SwaggerOperation("Get a user.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<UserJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetUser([FromRoute] string userId = "")
    {
        if (string.IsNullOrEmpty(userId) || !ObjectId.TryParse(userId, out ObjectId obj))
            return NotFound("Could not find user.");

        if (!((_DB.Teams.Cache.TryGetValue(Client.TeamId, out TeamData? Team) && Team.Members.ContainsKey(obj))))
            return NotFound("Could not find user.");

        AuthUser? user = await _DB.Run.GetCollection<AuthUser>("users").Find(new FilterDefinitionBuilder<AuthUser>().Eq(x => x.Id, obj)).FirstOrDefaultAsync();
        if (user == null)
            return NotFound("Could not find user.");

        if (Client.CheckFailedTeamPermissions(TeamPermission.ViewMembers, out TeamPermission? perm))
            return PermissionFailed(perm);

        return Ok(new UserJson(user));
    }
}
