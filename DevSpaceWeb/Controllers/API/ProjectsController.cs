using DevSpaceWeb.API;
using DevSpaceWeb.API.Projects;
using DevSpaceWeb.Data.Permissions;
using DevSpaceWeb.Data.Projects;
using DevSpaceWeb.Database;
using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
[SwaggerTag("Manage Team projects and information.")]
[IsAuthenticated]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized", typeof(ResponseUnauthorized))]
[SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden", typeof(ResponseForbidden))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ResponseBadRequest))]
public class ProjectsController : APIController
{
    [HttpGet("/api/projects/{projectId?}")]
    [SwaggerOperation("Get a project.", "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResponseData<ProjectJson>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(ResponseNotFound))]
    public async Task<IActionResult> GetProject([FromRoute] string projectId = "")
    {
        if (string.IsNullOrEmpty(projectId) || !ObjectId.TryParse(projectId, out ObjectId obj) || !_DB.Projects.Cache.TryGetValue(obj, out ProjectData? project) || !(project.TeamId == Client.TeamId))
            return NotFound("Could not find project.");

        if (Client.CheckFailedProjectPermissions(project, ProjectPermission.ViewProject, out ProjectPermission? perm))
            return PermissionFailed(perm!);

        return Ok(new ProjectJson(project));
    }
}
