using App.BLL.Commands;
using App.DTO.v1.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class SignUpController : Controller
{
    private readonly IMediator _mediator;
    private readonly SignInManager<App.Domain.Identity.AppUser> _signInManager;

    public SignUpController(IMediator mediator, SignInManager<App.Domain.Identity.AppUser> signInManager)
    {
        _mediator = mediator;
        _signInManager = signInManager;
    }

    // GET: /register
    [HttpGet]
    [Route("register")]
    public IActionResult Register()
    {
        return View(new SignUpSchoolInputModel());
    }

    // POST: /register
    [HttpPost]
    [Route("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(SignUpSchoolInputModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var command = new SignUpSchoolCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            // Sign in the user automatically
            var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            // Redirect to dashboard
            return Redirect($"/{result.Value!.Slug}/dashboard");
        }

        ModelState.AddModelError(string.Empty, result.Error!);
        return View(model);
    }
}
