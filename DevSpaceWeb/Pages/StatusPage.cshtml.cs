using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DevSpaceWeb.Pages;

public class StatusPageModel : PageModel
{
    public StatusPageModel(HealthCheckService health)
    {
        HealthService = health;
    }

    public HealthCheckService HealthService;

    public HealthReport Report = null!;

    public async Task OnGet()
    {
        Report = await HealthService.CheckHealthAsync();
    }
}
