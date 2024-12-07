using DevSpaceWeb.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DevSpaceWeb.Controllers.API;

[ShowInSwagger]
public class TeamController : Controller
{
    [HttpGet("/api/team/{teamId?}")]
    public async Task GetTeam([FromRoute] string teamId = "")
    {

    }

    [HttpPost("/api/team/{teamId?}")]
    public async Task EditTeam([FromRoute] string teamId = "")
    {

    }
}
