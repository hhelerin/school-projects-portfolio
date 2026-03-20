using App.BLL.Features.DanceStyles;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
[Route("{slug}/dance-styles")]
public class DanceStyleController : Controller
{
    private readonly IMediator _mediator;

    public DanceStyleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string slug)
    {
        var query = new GetListDanceStyleQuery();
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return View(new List<DanceStyleListItemDto>());
        }

        ViewData["Slug"] = slug;
        return View(result.Value);
    }

    [HttpGet("create")]
    public IActionResult Create(string slug)
    {
        ViewData["Slug"] = slug;
        return View(new CreateDanceStyleInputModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string slug, CreateDanceStyleInputModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Slug"] = slug;
            return View(model);
        }

        var command = new CreateDanceStyleCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Dance style created successfully!";
            return RedirectToAction(nameof(Index), new { slug });
        }

        ModelState.AddModelError(string.Empty, result.Error!);
        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpGet("{id}/edit")]
    public async Task<IActionResult> Edit(string slug, Guid id)
    {
        var query = new GetDanceStyleByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index), new { slug });
        }

        var model = new UpdateDanceStyleInputModel
        {
            Id = result.Value!.Id,
            Name = result.Value.Name,
            Details = result.Value.Details
        };

        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpPost("{id}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string slug, Guid id, UpdateDanceStyleInputModel model)
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

        var command = new UpdateDanceStyleCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Dance style updated successfully!";
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
        var command = new DeleteDanceStyleCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Dance style deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }
}