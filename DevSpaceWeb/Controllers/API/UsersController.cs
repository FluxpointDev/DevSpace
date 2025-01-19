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
            return BadRequest("Could not find user.");

        if (!(Client.IsInstanceAdmin || (_DB.Teams.Cache.TryGetValue(Client.TeamId.GetValueOrDefault(), out TeamData Team) && Team.Members.ContainsKey(obj))))
            return BadRequest("Could not find user.");

        AuthUser? user = await _DB.Run.GetCollection<AuthUser>("users").Find(new FilterDefinitionBuilder<AuthUser>().Eq(x => x.Id, obj)).FirstOrDefaultAsync();
        if (user == null)
            return BadRequest("Could not find user.");

        if (Client.CheckFailedTeamPermissions(TeamPermission.ViewMembers, out var perm))
            return PermissionFailed(perm);

        return Ok(new UserJson(user));
    }
}
