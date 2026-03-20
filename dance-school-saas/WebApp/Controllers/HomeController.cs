using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using App.DAL.EF;
using App.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private static int _counter = 0;

    public HomeController(
        AppDbContext context,
        ILogger<HomeController> logger,
        UserManager<AppUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel();

        // Check if User is available (null in some test scenarios without HttpContext)
        if (User?.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                viewModel.IsAuthenticated = true;
                viewModel.UserFirstName = user.FirstName;
                viewModel.IsRootUser = User.IsInRole("SystemAdmin");

                var userCompanies = await _context.CompanyUsers
                    .AsNoTracking()
                    .Include(cu => cu.Company)
                    .Where(cu => cu.AppUserId == user.Id && cu.IsActive)
                    .Select(cu => cu.Company)
                    .ToListAsync();

                viewModel.UserCompanies = userCompanies
                    .Where(c => c != null)
                    .Select(c => new CompanySummaryViewModel
                    {
                        Name = c!.Name,
                        Slug = c!.Slug
                    })
                    .ToList();
            }
        }

        return View(viewModel);
    }

    public async Task<string> HtmxClicked()
    {
        _counter++;
        return "Htmx Click Me - " + _counter;
    }


    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        try
        {
            var reqCulture = new RequestCulture(culture);

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(reqCulture),
                new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                }
            );
        }
        catch (Exception e)
        {
            _logger.LogError("SetLanguage exception: {}", e.Message);
        }

        return LocalRedirect(returnUrl);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}