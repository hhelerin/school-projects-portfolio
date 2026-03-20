using App.BLL.Features.Instructors;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager")]
[Route("{slug}/instructors")]
public class InstructorController : Controller
{
    private readonly IMediator _mediator;

    public InstructorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string slug)
    {
        var query = new GetListInstructorQuery();
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return View(new List<InstructorListItemDto>());
        }

        ViewData["Slug"] = slug;
        return View(result.Value);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(string slug)
    {
        var usersQuery = new GetCompanyUsersForDropdownQuery();
        var usersResult = await _mediator.Send(usersQuery);

        ViewData["Slug"] = slug;
        ViewData["CompanyUsers"] = usersResult.IsSuccess ? usersResult.Value : new List<CompanyUserDropdownDto>();
        return View(new CreateInstructorInputModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string slug, CreateInstructorInputModel model)
    {
        if (!ModelState.IsValid)
        {
            var usersQuery = new GetCompanyUsersForDropdownQuery();
            var usersResult = await _mediator.Send(usersQuery);
            ViewData["CompanyUsers"] = usersResult.IsSuccess ? usersResult.Value : new List<CompanyUserDropdownDto>();
            ViewData["Slug"] = slug;
            return View(model);
        }

        var command = new CreateInstructorCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Instructor created successfully!";
            return RedirectToAction(nameof(Index), new { slug });
        }

        ModelState.AddModelError(string.Empty, result.Error!);
        var usersQuery2 = new GetCompanyUsersForDropdownQuery();
        var usersResult2 = await _mediator.Send(usersQuery2);
        ViewData["CompanyUsers"] = usersResult2.IsSuccess ? usersResult2.Value : new List<CompanyUserDropdownDto>();
        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpGet("{id}/edit")]
    public async Task<IActionResult> Edit(string slug, Guid id)
    {
        var query = new GetInstructorByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index), new { slug });
        }

        var model = new UpdateInstructorInputModel
        {
            Id = result.Value!.Id,
            Name = result.Value.Name,
            PersonalId = result.Value.PersonalId,
            ContactInfo = result.Value.ContactInfo,
            AppUserId = result.Value.AppUserId,
            Details = result.Value.Details
        };

        var usersQuery = new GetCompanyUsersForDropdownQuery();
        var usersResult = await _mediator.Send(usersQuery);
        ViewData["CompanyUsers"] = usersResult.IsSuccess ? usersResult.Value : new List<CompanyUserDropdownDto>();
        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpPost("{id}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string slug, Guid id, UpdateInstructorInputModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            var usersQuery = new GetCompanyUsersForDropdownQuery();
            var usersResult = await _mediator.Send(usersQuery);
            ViewData["CompanyUsers"] = usersResult.IsSuccess ? usersResult.Value : new List<CompanyUserDropdownDto>();
            ViewData["Slug"] = slug;
            return View(model);
        }

        var command = new UpdateInstructorCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Instructor updated successfully!";
            return RedirectToAction(nameof(Index), new { slug });
        }

        ModelState.AddModelError(string.Empty, result.Error!);
        var usersQuery2 = new GetCompanyUsersForDropdownQuery();
        var usersResult2 = await _mediator.Send(usersQuery2);
        ViewData["CompanyUsers"] = usersResult2.IsSuccess ? usersResult2.Value : new List<CompanyUserDropdownDto>();
        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpPost("{id}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string slug, Guid id)
    {
        var command = new DeleteInstructorCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Instructor deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }
}