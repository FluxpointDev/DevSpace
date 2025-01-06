using DevSpaceWeb.API.Users;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Data.Users;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[IsAuthenticated]
public class UserController : APIController
{
    [HttpGet("/api/users/{userId?}")]
    public async Task<IActionResult> GetUser([FromRoute] string userId = "")
    {
        if (string.IsNullOrEmpty(userId) || !ObjectId.TryParse(userId, out ObjectId obj) || !_DB.Teams.Cache.TryGetValue(Client.TeamId.Value, out TeamData Team) || !Team.Members.ContainsKey(obj))
            return BadRequest("Could not find user.");

        AuthUser? user = await _DB.Run.GetCollection<AuthUser>("users").Find<AuthUser>(new FilterDefinitionBuilder<AuthUser>().Eq(x => x.Id, obj)).FirstOrDefaultAsync();
        if (user == null)
            return BadRequest("Could not find user.");

        return Ok(new UserJson(user));
    }
}
