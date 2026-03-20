using App.BLL.Features.Studios;
using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

[CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
[Route("{slug}/studios")]
public class StudioController : Controller
{
    private readonly IMediator _mediator;
    private readonly AppDbContext _context;

    public StudioController(IMediator mediator, AppDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string slug)
    {
        var query = new GetListStudioQuery();
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return View(new List<StudioListItemDto>());
        }

        ViewData["Slug"] = slug;
        return View(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Detail(string slug, Guid id)
    {
        var query = new GetStudioByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index), new { slug });
        }

        ViewData["Slug"] = slug;
        return View(result.Value);
    }

    [HttpGet("create")]
    public IActionResult Create(string slug)
    {
        ViewData["Slug"] = slug;
        return View(new CreateStudioInputModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string slug, CreateStudioInputModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Slug"] = slug;
            return View(model);
        }

        var command = new CreateStudioCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Studio created successfully!";
            return RedirectToAction(nameof(Index), new { slug });
        }

        ModelState.AddModelError(string.Empty, result.Error!);
        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpGet("{id}/edit")]
    public async Task<IActionResult> Edit(string slug, Guid id)
    {
        var query = new GetStudioByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index), new { slug });
        }

        var model = new UpdateStudioInputModel
        {
            Id = result.Value!.Id,
            Name = result.Value.Name,
            Details = result.Value.Details,
            ContactInfo = result.Value.ContactInfo
        };

        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpPost("{id}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string slug, Guid id, UpdateStudioInputModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Slug"] = slug;
            return View(model);
        }

        var command = new UpdateStudioCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Studio updated successfully!";
            return RedirectToAction(nameof(Index), new { slug });
        }

        ModelState.AddModelError(string.Empty, result.Error!);
        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpPost("{id}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string slug, Guid id)
    {
        var command = new DeleteStudioCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Studio deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }
}