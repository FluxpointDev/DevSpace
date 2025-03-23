using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DevSpaceWeb.Pages;

public class StatusPageModel : PageModel
{
    public StatusPageModel(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService health)
    {
        HealthService = health;
    }

    public Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService HealthService;

    public HealthReport Report;

    public async Task OnGet()
    {
        Report = await HealthService.CheckHealthAsync();
    }
}
