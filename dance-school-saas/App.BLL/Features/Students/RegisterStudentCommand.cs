using App.DAL.EF;
using App.Domain;
using App.DTO.v1.Students;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Students;

public record RegisterStudentCommand(RegisterStudentInputModel Model) : IRequest<Result<Guid>>;

public class RegisterStudentCommandValidator : AbstractValidator<RegisterStudentCommand>
{
    public RegisterStudentCommandValidator()
    {
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

public class RegisterStudentCommandHandler : IRequestHandler<RegisterStudentCommand, Result<Guid>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public RegisterStudentCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<Guid>> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
    {
        // If AppUserId provided, verify user exists and is not already a student in this tenant
        if (request.Model.AppUserId.HasValue)
        {
            var userExists = await _context.CompanyUsers
                .AnyAsync(cu => cu.AppUserId == request.Model.AppUserId.Value && cu.CompanyId == _tenantContext.CompanyId, cancellationToken);

            if (!userExists)
            {
                return Result<Guid>.Failure("Selected user is not a member of this company");
            }

            var userIsAlreadyStudent = await _context.Students
                .AnyAsync(s => s.AppUserId == request.Model.AppUserId.Value && s.CompanyId == _tenantContext.CompanyId && !s.IsDeleted, cancellationToken);

            if (userIsAlreadyStudent)
            {
                return Result<Guid>.Failure("This user is already linked to another student in this company");
            }
        }

        // Duplicate detection: check for same Name AND ContactInfo in same tenant
        if (!request.Model.ConfirmDuplicate)
        {
            var duplicateExists = await _context.Students
                .AnyAsync(s => 
                    s.CompanyId == _tenantContext.CompanyId && 
                    !s.IsDeleted &&
                    s.Name.ToLower() == request.Model.Name.ToLower() &&
                    s.ContactInfo != null && 
                    request.Model.ContactInfo != null &&
                    s.ContactInfo.ToLower() == request.Model.ContactInfo.ToLower(), 
                    cancellationToken);

            if (duplicateExists)
            {
                return Result<Guid>.Failure("A student with the same name and contact information already exists. Please confirm to save anyway.");
            }
        }

        var student = new Student
        {
            CompanyId = _tenantContext.CompanyId,
            Name = request.Model.Name,
            PersonalId = request.Model.PersonalId,
            ContactInfo = request.Model.ContactInfo,
            Details = request.Model.Details,
            AppUserId = request.Model.AppUserId
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(student.Id);
    }
}
