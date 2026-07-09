using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGERI.Web.Services;

namespace SIGERI.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IAnalyticsService _analyticsService;

    public HomeController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        Guid? userId = null;
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(claimValue, out var parsedUserId))
        {
            userId = parsedUserId;
        }

        var model = await _analyticsService.GetAnalyticsViewModelAsync(userId);
        return View(model);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Error()
    {
        return Problem();
    }
}

