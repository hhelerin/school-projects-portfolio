using App.DAL.EF;
using App.DTO.v1.Students;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Students;

public record UpdateStudentCommand(UpdateStudentInputModel Model) : IRequest<Result<StudentDetailDto>>;

public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
    {
        RuleFor(x => x.Model.StudentId)
            .NotEmpty().WithMessage("Student ID is required");
            
        RuleFor(x => x.Model.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
            
        RuleFor(x => x.Model.PersonalId)
            .MaximumLength(50).WithMessage("Personal ID must not exceed 50 characters");
            
        RuleFor(x => x.Model.ContactInfo)
            .MaximumLength(500).WithMessage("Contact info must not exceed 500 characters");
            
        RuleFor(x => x.Model.Details)
            .MaximumLength(1000).WithMessage("Details must not exceed 1000 characters");
    }
}

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Result<StudentDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public UpdateStudentCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<StudentDetailDto>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == request.Model.StudentId && s.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (student == null)
        {
            return Result<StudentDetailDto>.Failure("Student not found");
        }

        // If AppUserId changed, validate new AppUser exists and is not already linked to another student
        if (request.Model.AppUserId.HasValue && request.Model.AppUserId != student.AppUserId)
        {
            var userExists = await _context.CompanyUsers
                .AnyAsync(cu => cu.AppUserId == request.Model.AppUserId.Value && cu.CompanyId == _tenantContext.CompanyId, cancellationToken);

            if (!userExists)
            {
                return Result<StudentDetailDto>.Failure("Selected user is not a member of this company");
            }

            var userIsAlreadyStudent = await _context.Students
                .AnyAsync(s => s.AppUserId == request.Model.AppUserId.Value && s.CompanyId == _tenantContext.CompanyId && !s.IsDeleted && s.Id != request.Model.StudentId, cancellationToken);

            if (userIsAlreadyStudent)
            {
                return Result<StudentDetailDto>.Failure("This user is already linked to another student in this company");
            }
        }

        student.Name = request.Model.Name;
        student.PersonalId = request.Model.PersonalId;
        student.ContactInfo = request.Model.ContactInfo;
        student.AppUserId = request.Model.AppUserId;
        student.Details = request.Model.Details;
        student.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Get AppUser full name if linked
        string? appUserFullName = null;
        if (student.AppUserId.HasValue)
        {
            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == student.AppUserId.Value, cancellationToken);
            appUserFullName = appUser != null ? $"{appUser.FirstName} {appUser.LastName}".Trim() : null;
        }

        return Result<StudentDetailDto>.Success(new StudentDetailDto
        {
            Id = student.Id,
            Name = student.Name,
            PersonalId = student.PersonalId,
            ContactInfo = student.ContactInfo,
            Details = student.Details,
            AppUserId = student.AppUserId,
            AppUserFullName = appUserFullName,
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt
        });
    }
}
