using App.BLL.Features.StudioRooms;
using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using App.Helpers.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

[CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
[Route("{slug}/studios/{studioId}/rooms")]
public class StudioRoomController : Controller
{
    private readonly IMediator _mediator;
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public StudioRoomController(IMediator mediator, AppDbContext context, ITenantContext tenantContext)
    {
        _mediator = mediator;
        _context = context;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string slug, Guid studioId)
    {
        var query = new GetRoomsByStudioQuery(studioId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction("Detail", "Studio", new { slug, id = studioId });
        }

        ViewData["Slug"] = slug;
        ViewData["StudioId"] = studioId;

        var studio = await _context.Studios
            .FirstOrDefaultAsync(s => s.Id == studioId && s.CompanyId == _tenantContext.CompanyId);
        ViewData["StudioName"] = studio?.Name ?? "Unknown";

        return View(result.Value);
    }

    [HttpGet("create")]
    public IActionResult Create(string slug, Guid studioId)
    {
        ViewData["Slug"] = slug;
        ViewData["StudioId"] = studioId;
        return View(new CreateStudioRoomInputModel { StudioId = studioId });
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string slug, Guid studioId, CreateStudioRoomInputModel model)
    {
        if (studioId != model.StudioId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Slug"] = slug;
            ViewData["StudioId"] = studioId;
            return View(model);
        }

        var command = new CreateStudioRoomCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Room created successfully!";
            return RedirectToAction(nameof(Index), new { slug, studioId });
        }

        ModelState.AddModelError(string.Empty, result.Error!);
        ViewData["Slug"] = slug;
        ViewData["StudioId"] = studioId;
        return View(model);
    }

    [HttpGet("{id}/edit")]
    public async Task<IActionResult> Edit(string slug, Guid studioId, Guid id)
    {
        var query = new GetStudioRoomByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index), new { slug, studioId });
        }

        var model = new UpdateStudioRoomInputModel
        {
            Id = result.Value!.Id,
            Name = result.Value.Name,
            Details = result.Value.Details
        };

        ViewData["Slug"] = slug;
        ViewData["StudioId"] = studioId;
        ViewData["Features"] = result.Value.Features;

        // Get all available features for dropdown
        var features = await _context.Features
            .OrderBy(f => f.Name)
            .Select(f => new FeatureDto { Id = f.Id, Name = f.Name })
            .ToListAsync();
        ViewData["AvailableFeatures"] = features;

        return View(model);
    }

    [HttpPost("{id}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string slug, Guid studioId, Guid id, UpdateStudioRoomInputModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Slug"] = slug;
            ViewData["StudioId"] = studioId;
            return View(model);
        }

        var command = new UpdateStudioRoomCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Room updated successfully!";
            return RedirectToAction(nameof(Index), new { slug, studioId });
        }

        ModelState.AddModelError(string.Empty, result.Error!);
        ViewData["Slug"] = slug;
        ViewData["StudioId"] = studioId;
        return View(model);
    }

    [HttpPost("{id}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string slug, Guid studioId, Guid id)
    {
        var command = new DeleteStudioRoomCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Room deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
        }

        return RedirectToAction(nameof(Index), new { slug, studioId });
    }

    [HttpPost("{id}/add-feature")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFeature(string slug, Guid studioId, Guid id, AddStudioFeatureInputModel model)
    {
        if (id != model.StudioRoomId)
        {
            return BadRequest();
        }

        var command = new AddStudioFeatureCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Feature added successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
        }

        return RedirectToAction(nameof(Edit), new { slug, studioId, id });
    }

    [HttpPost("remove-feature/{studioFeatureId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFeature(string slug, Guid studioId, Guid id, Guid studioFeatureId)
    {
        var command = new RemoveStudioFeatureCommand(studioFeatureId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Feature removed successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
        }

        return RedirectToAction(nameof(Edit), new { slug, studioId, id });
    }
}