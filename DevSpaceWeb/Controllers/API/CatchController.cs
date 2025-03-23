using Microsoft.AspNetCore.Mvc;

namespace DevSpaceWeb.Controllers.API;

public class CatchController : APIController
{
    [Route("/api/{*all}")]
    public IActionResult CatchAll()
    {
        return NotFound("This API endpoint does not exist.");
    }

    [HttpGet("/api/teapot")]
    public IActionResult Teapot()
    {
        return CustomStatus(418, "I'm a little teapot, Short and stout, Here is my handle, Here is my spout.");
    }


    [HttpGet("/api/test/code/{code:int?}")]
    public IActionResult TestCode([FromRoute] int code = 0)
    {
        switch (code)
        {
            case 200:
                return Ok("This is a success :)");
            case 400:
                return BadRequest();
            case 404:
                return NotFound();
            case 401:
                return Unauthorized();
            case 500:
                return InternalServerError();
            case 403:
                return Forbidden();
            case 501:
                throw new Exception("Error occured");
        }

        return BadRequest("Invalid code");
    }
}
