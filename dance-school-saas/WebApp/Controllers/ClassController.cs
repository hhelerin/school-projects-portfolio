using App.BLL.Features.Classes;
using App.BLL.Features.ClassSchedules;
using App.DAL.EF;
using App.DTO.v1.ClassScheduling;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

[CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager", "CompanyEmployee")]
[Route("{slug}/classes")]
public class ClassController : Controller
{
    private readonly IMediator _mediator;
    private readonly AppDbContext _context;

    public ClassController(IMediator mediator, AppDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string slug)
    {
        var query = new GetClassListQuery();
        var result = await _mediator.Send(query);

        ViewData["Slug"] = slug;
        return View(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Detail(string slug, Guid id)
    {
        var query = new GetClassByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
        {
            TempData["ErrorMessage"] = "Class not found";
            return RedirectToAction(nameof(Index), new { slug });
        }

        var schedulesQuery = new GetClassSchedulesByClassQuery(id);
        var schedules = await _mediator.Send(schedulesQuery);
        
        ViewData["Slug"] = slug;
        ViewData["Schedules"] = schedules;
        return View(result);
    }

    [HttpGet("create")]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
    public async Task<IActionResult> Create(string slug)
    {
        var model = new CreateClassInputModel
        {
            RecurrenceStartDate = DateOnly.FromDateTime(DateTime.Today),
            RecurrenceEndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(3))
        };

        ViewData["Slug"] = slug;
        await PopulateDropdowns();
        return View(model);
    }

    [HttpPost("create")]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string slug, CreateClassInputModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Slug"] = slug;
            await PopulateDropdowns();
            return View(model);
        }

        var command = new CreateClassCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Class created successfully!";
            return RedirectToAction(nameof(Index), new { slug });
        }

        // Check for conflict error
        if (result.Error != null && result.Error.Contains("conflict"))
        {
            ModelState.AddModelError(string.Empty, result.Error);
        }
        else
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "An error occurred");
        }

        ViewData["Slug"] = slug;
        await PopulateDropdowns();
        return View(model);
    }

    [HttpGet("{id}/edit")]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
    public async Task<IActionResult> Edit(string slug, Guid id)
    {
        var query = new GetClassByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
        {
            TempData["ErrorMessage"] = "Class not found";
            return RedirectToAction(nameof(Index), new { slug });
        }

        var model = new UpdateClassInputModel
        {
            ClassId = result.Id,
            Name = result.Name,
            Details = result.Details,
            Capacity = result.Capacity
        };

        ViewData["Slug"] = slug;
        ViewData["Note"] = "Note: Schedule changes must be done per-occurrence from the schedule view.";
        return View(model);
    }

    [HttpPost("{id}/edit")]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string slug, Guid id, UpdateClassInputModel model)
    {
        if (id != model.ClassId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Slug"] = slug;
            return View(model);
        }

        var command = new UpdateClassCommand(model.ClassId, model.Name, model.Details, model.Capacity);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Class updated successfully!";
            return RedirectToAction(nameof(Index), new { slug });
        }

        ModelState.AddModelError(string.Empty, result.Error ?? "An error occurred");
        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpPost("{id}/delete")]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string slug, Guid id)
    {
        var command = new DeleteClassCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Class deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    private async Task PopulateDropdowns()
    {
        var rooms = await _context.StudioRooms
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Name)
            .ToListAsync();
        ViewData["StudioRooms"] = rooms.Select(r => new { r.Id, r.Name });

        var instructors = await _context.Instructors
            .Where(i => !i.IsDeleted)
            .OrderBy(i => i.Name)
            .ToListAsync();
        ViewData["Instructors"] = instructors.Select(i => new { i.Id, i.Name });

        var styles = await _context.DanceStyles
            .Where(d => !d.IsDeleted)
            .OrderBy(d => d.Name)
            .ToListAsync();
        ViewData["DanceStyles"] = styles.Select(s => new { s.Id, s.Name });

        var levels = await _context.Levels
            .Where(l => !l.IsDeleted)
            .OrderBy(l => l.Name)
            .ToListAsync();
        ViewData["Levels"] = levels.Select(l => new { l.Id, l.Name });
    }
}
