using App.BLL.Features.ClassSchedules;
using App.DAL.EF;
using App.DTO.v1.ClassScheduling;
using App.Helpers;
using App.Helpers.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

[CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager", "CompanyEmployee")]
[Route("{slug}/schedule")]
public class ClassScheduleController : Controller
{
    private readonly IMediator _mediator;
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public ClassScheduleController(IMediator mediator, AppDbContext context, ITenantContext tenantContext)
    {
        _mediator = mediator;
        _context = context;
        _tenantContext = tenantContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string slug, DateOnly? weekStart, Guid? styleId, Guid? levelId, Guid? roomId, Guid? instructorId)
    {
        var weekStartDate = weekStart ?? DateOnly.FromDateTime(DateTime.Today);
        
        // Adjust to Monday of the week
        var daysToMonday = ((int)weekStartDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var monday = weekStartDate.AddDays(-daysToMonday);

        var query = new GetScheduleByWeekQuery(monday, styleId, levelId, roomId, instructorId);
        var schedules = await _mediator.Send(query);

        ViewData["Slug"] = slug;
        ViewData["WeekStart"] = monday;
        ViewData["WeekEnd"] = monday.AddDays(6);
        ViewData["DanceStyleId"] = styleId;
        ViewData["LevelId"] = levelId;
        ViewData["StudioRoomId"] = roomId;
        ViewData["InstructorId"] = instructorId;
        
        await PopulateFilters();
        return View(schedules);
    }

    [HttpGet("{id}/edit")]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
    public async Task<IActionResult> Edit(string slug, Guid id)
    {
        var schedule = await _context.ClassSchedules
            .Where(cs => cs.Id == id && cs.CompanyId == _tenantContext.CompanyId)
            .Include(cs => cs.Class)
            .FirstOrDefaultAsync();

        if (schedule == null)
        {
            TempData["ErrorMessage"] = "Schedule not found";
            return RedirectToAction(nameof(Index), new { slug });
        }

        var model = new EditClassScheduleInstanceInputModel
        {
            ClassScheduleId = schedule.Id,
            Date = schedule.Date,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            StudioRoomId = schedule.StudioRoomId,
            Details = schedule.Details
        };

        ViewData["Slug"] = slug;
        ViewData["OriginalDate"] = schedule.Date;
        ViewData["OriginalStartTime"] = schedule.StartTime;
        ViewData["OriginalEndTime"] = schedule.EndTime;
        ViewData["OriginalRoomId"] = schedule.StudioRoomId;
        await PopulateRoomDropdown();
        return View(model);
    }

    [HttpPost("{id}/edit")]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string slug, Guid id, EditClassScheduleInstanceInputModel model)
    {
        if (id != model.ClassScheduleId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Slug"] = slug;
            await PopulateRoomDropdown();
            return View(model);
        }

        var command = new EditClassScheduleInstanceCommand(
            model.ClassScheduleId,
            model.Date,
            model.StartTime,
            model.EndTime,
            model.StudioRoomId,
            model.Details
        );
        
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Schedule updated successfully!";
            return RedirectToAction(nameof(Index), new { slug });
        }

        ModelState.AddModelError(string.Empty, result.Error ?? "An error occurred");
        ViewData["Slug"] = slug;
        await PopulateRoomDropdown();
        return View(model);
    }

    [HttpPost("{id}/cancel")]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(string slug, Guid id, string cancellationReason)
    {
        var command = new CancelClassScheduleInstanceCommand(id, cancellationReason);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Class cancelled successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    private async Task PopulateFilters()
    {
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
    }

    private async Task PopulateRoomDropdown()
    {
        var rooms = await _context.StudioRooms
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Name)
            .ToListAsync();
        ViewData["StudioRooms"] = rooms.Select(r => new { r.Id, r.Name });
    }
}
