using DevSpaceWeb.API;
using DevSpaceWeb.API.Teams;
using DevSpaceWeb.API.Users;
using DevSpaceWeb.Data.Teams;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class MembersController : APIController
{
    [HttpGet("/api/members/{memberId?}")]
    [SwaggerOperation("Get a member.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(MemberJson))]
    public async Task<IActionResult> GetMember([FromRoute] string memberId = "")
    {
        if (string.IsNullOrEmpty(memberId) || !ObjectId.TryParse(memberId, out ObjectId obj) || !_DB.Members.Cache.TryGetValue(obj, out TeamMemberData member) || member.TeamId != Client.TeamId)
            return BadRequest("Could not find member.");

        return Ok(new MemberJson(member));
    }

    

    [HttpGet("/api/teams/{teamId?}/members")]
    [SwaggerOperation("Get a list of members.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(MemberJson[]))]
    public async Task GetMembers([FromRoute] string teamId = "")
    {

    }
}
