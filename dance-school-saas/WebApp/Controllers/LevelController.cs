using App.BLL.Features.Levels;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
[Route("{slug}/levels")]
public class LevelController : Controller
{
    private readonly IMediator _mediator;

    public LevelController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string slug)
    {
        var query = new GetListLevelQuery();
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return View(new List<LevelListItemDto>());
        }

        ViewData["Slug"] = slug;
        return View(result.Value);
    }

    [HttpGet("create")]
    public IActionResult Create(string slug)
    {
        ViewData["Slug"] = slug;
        return View(new CreateLevelInputModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string slug, CreateLevelInputModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Slug"] = slug;
            return View(model);
        }

        var command = new CreateLevelCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Level created successfully!";
            return RedirectToAction(nameof(Index), new { slug });
        }

        ModelState.AddModelError(string.Empty, result.Error!);
        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpGet("{id}/edit")]
    public async Task<IActionResult> Edit(string slug, Guid id)
    {
        var query = new GetLevelByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index), new { slug });
        }

        var model = new UpdateLevelInputModel
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
    public async Task<IActionResult> Edit(string slug, Guid id, UpdateLevelInputModel model)
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

        var command = new UpdateLevelCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Level updated successfully!";
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
        var command = new DeleteLevelCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Level deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }
}