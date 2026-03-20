using App.DAL.EF;
using App.Domain;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Instructors;

public record CreateInstructorCommand(CreateInstructorInputModel Model) : IRequest<Result<InstructorDetailDto>>;

public class CreateInstructorCommandValidator : AbstractValidator<CreateInstructorCommand>
{
    public CreateInstructorCommandValidator()
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

public class CreateInstructorCommandHandler : IRequestHandler<CreateInstructorCommand, Result<InstructorDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateInstructorCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<InstructorDetailDto>> Handle(CreateInstructorCommand request, CancellationToken cancellationToken)
    {
        // If AppUserId provided, verify user belongs to this company
        if (request.Model.AppUserId.HasValue)
        {
            var userExists = await _context.CompanyUsers
                .AnyAsync(cu => cu.AppUserId == request.Model.AppUserId.Value && cu.CompanyId == _tenantContext.CompanyId, cancellationToken);

            if (!userExists)
            {
                return Result<InstructorDetailDto>.Failure("Selected user is not a member of this company");
            }
        }

        var instructor = new Instructor
        {
            CompanyId = _tenantContext.CompanyId,
            Name = request.Model.Name,
            PersonalId = request.Model.PersonalId,
            ContactInfo = request.Model.ContactInfo,
            AppUserId = request.Model.AppUserId,
            Details = request.Model.Details
        };

        _context.Instructors.Add(instructor);
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