using App.BLL.Features.Students;
using App.DTO.v1.Students;
using App.Helpers.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager", "CompanyEmployee")]
[Route("{slug}/students")]
public class StudentController : Controller
{
    private readonly IMediator _mediator;

    public StudentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string slug, string? searchTerm, int pageNumber = 1, int pageSize = 20)
    {
        var query = new GetStudentListQuery(searchTerm, pageNumber, pageSize);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess || result.Value == null)
        {
            TempData["ErrorMessage"] = result.Error;
            return View(new List<StudentListItemDto>());
        }

        ViewData["Slug"] = slug;
        ViewData["SearchTerm"] = searchTerm;
        return View(result.Value.Items);
    }

    [HttpGet("create")]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager", "CompanyEmployee")]
    public async Task<IActionResult> Create(string slug)
    {
        var usersQuery = new App.BLL.Features.Instructors.GetCompanyUsersForDropdownQuery();
        var usersResult = await _mediator.Send(usersQuery);

        ViewData["Slug"] = slug;
        ViewData["CompanyUsers"] = usersResult.IsSuccess ? usersResult.Value : new List<App.DTO.v1.SchoolConfiguration.CompanyUserDropdownDto>();
        return View(new RegisterStudentInputModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager", "CompanyEmployee")]
    public async Task<IActionResult> Create(string slug, RegisterStudentInputModel model)
    {
        if (!ModelState.IsValid)
        {
            var usersQuery = new App.BLL.Features.Instructors.GetCompanyUsersForDropdownQuery();
            var usersResult = await _mediator.Send(usersQuery);
            ViewData["CompanyUsers"] = usersResult.IsSuccess ? usersResult.Value : new List<App.DTO.v1.SchoolConfiguration.CompanyUserDropdownDto>();
            ViewData["Slug"] = slug;
            return View(model);
        }

        var command = new RegisterStudentCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Student registered successfully!";
            return RedirectToAction(nameof(Detail), new { slug, id = result.Value });
        }

        // Check if it's a duplicate warning
        if (result.Error != null && result.Error.Contains("already exists"))
        {
            ModelState.AddModelError(string.Empty, result.Error);
            var usersQuery = new App.BLL.Features.Instructors.GetCompanyUsersForDropdownQuery();
            var usersResult = await _mediator.Send(usersQuery);
            ViewData["CompanyUsers"] = usersResult.IsSuccess ? usersResult.Value : new List<App.DTO.v1.SchoolConfiguration.CompanyUserDropdownDto>();
            ViewData["Slug"] = slug;
            ViewData["ShowDuplicateWarning"] = true;
            return View(model);
        }

        ModelState.AddModelError(string.Empty, result.Error!);
        var usersQuery2 = new App.BLL.Features.Instructors.GetCompanyUsersForDropdownQuery();
        var usersResult2 = await _mediator.Send(usersQuery2);
        ViewData["CompanyUsers"] = usersResult2.IsSuccess ? usersResult2.Value : new List<App.DTO.v1.SchoolConfiguration.CompanyUserDropdownDto>();
        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Detail(string slug, Guid id)
    {
        var query = new GetStudentByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index), new { slug });
        }

        ViewData["Slug"] = slug;
        return View(result.Value);
    }

    [HttpGet("{id}/edit")]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager", "CompanyEmployee")]
    public async Task<IActionResult> Edit(string slug, Guid id)
    {
        var query = new GetStudentByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index), new { slug });
        }

        var model = new UpdateStudentInputModel
        {
            StudentId = result.Value!.Id,
            Name = result.Value.Name,
            PersonalId = result.Value.PersonalId,
            ContactInfo = result.Value.ContactInfo,
            Details = result.Value.Details,
            AppUserId = result.Value.AppUserId
        };

        var usersQuery = new App.BLL.Features.Instructors.GetCompanyUsersForDropdownQuery();
        var usersResult = await _mediator.Send(usersQuery);
        ViewData["CompanyUsers"] = usersResult.IsSuccess ? usersResult.Value : new List<App.DTO.v1.SchoolConfiguration.CompanyUserDropdownDto>();
        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpPost("{id}/edit")]
    [ValidateAntiForgeryToken]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin", "CompanyManager", "CompanyEmployee")]
    public async Task<IActionResult> Edit(string slug, Guid id, UpdateStudentInputModel model)
    {
        if (id != model.StudentId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            var usersQuery = new App.BLL.Features.Instructors.GetCompanyUsersForDropdownQuery();
            var usersResult = await _mediator.Send(usersQuery);
            ViewData["CompanyUsers"] = usersResult.IsSuccess ? usersResult.Value : new List<App.DTO.v1.SchoolConfiguration.CompanyUserDropdownDto>();
            ViewData["Slug"] = slug;
            return View(model);
        }

        var command = new UpdateStudentCommand(model);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Student updated successfully!";
            return RedirectToAction(nameof(Detail), new { slug, id });
        }

        ModelState.AddModelError(string.Empty, result.Error!);
        var usersQuery2 = new App.BLL.Features.Instructors.GetCompanyUsersForDropdownQuery();
        var usersResult2 = await _mediator.Send(usersQuery2);
        ViewData["CompanyUsers"] = usersResult2.IsSuccess ? usersResult2.Value : new List<App.DTO.v1.SchoolConfiguration.CompanyUserDropdownDto>();
        ViewData["Slug"] = slug;
        return View(model);
    }

    [HttpPost("{id}/delete")]
    [ValidateAntiForgeryToken]
    [CompanyRoleAuthorize("CompanyOwner", "CompanyAdmin")]
    public async Task<IActionResult> Delete(string slug, Guid id)
    {
        var command = new DeleteStudentCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Student deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Detail), new { slug, id });
        }

        return RedirectToAction(nameof(Index), new { slug });
    }
}
