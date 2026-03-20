using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Instructors;

public record UpdateInstructorCommand(UpdateInstructorInputModel Model) : IRequest<Result<InstructorDetailDto>>;

public class UpdateInstructorCommandValidator : AbstractValidator<UpdateInstructorCommand>
{
    public UpdateInstructorCommandValidator()
    {
        RuleFor(x => x.Model.Id)
            .NotEmpty().WithMessage("Id is required");
            
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

public class UpdateInstructorCommandHandler : IRequestHandler<UpdateInstructorCommand, Result<InstructorDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public UpdateInstructorCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<InstructorDetailDto>> Handle(UpdateInstructorCommand request, CancellationToken cancellationToken)
    {
        var instructor = await _context.Instructors
            .FirstOrDefaultAsync(i => i.Id == request.Model.Id && i.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (instructor == null)
        {
            return Result<InstructorDetailDto>.Failure("Instructor not found");
        }

        // If AppUserId provided, verify user belongs to this company
        if (request.Model.AppUserId.HasValue && request.Model.AppUserId != instructor.AppUserId)
        {
            var userExists = await _context.CompanyUsers
                .AnyAsync(cu => cu.AppUserId == request.Model.AppUserId.Value && cu.CompanyId == _tenantContext.CompanyId, cancellationToken);

            if (!userExists)
            {
                return Result<InstructorDetailDto>.Failure("Selected user is not a member of this company");
            }
        }

        instructor.Name = request.Model.Name;
        instructor.PersonalId = request.Model.PersonalId;
        instructor.ContactInfo = request.Model.ContactInfo;
        instructor.AppUserId = request.Model.AppUserId;
        instructor.Details = request.Model.Details;
        instructor.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<InstructorDetailDto>.Success(new InstructorDetailDto
        {
            Id = instructor.Id,
            Name = instructor.Name,
            PersonalId = instructor.PersonalId,
            ContactInfo = instructor.ContactInfo,
            AppUserId = instructor.AppUserId,
            Details = instructor.Details
        });
    }
}