using Microsoft.AspNetCore.Mvc;

namespace DevSpaceWeb.Controllers;
public class TestController : Controller
{
    [HttpGet("/api/test/headers")]
    public async Task<IActionResult> Headers()
    {
        Console.WriteLine("---");
        foreach (var i in Request.Headers)
        {
            Console.WriteLine($"Header: {i.Key} - {i.Value.ToString()}");
        }
        Console.WriteLine("---");
        return Ok();
    }
}
