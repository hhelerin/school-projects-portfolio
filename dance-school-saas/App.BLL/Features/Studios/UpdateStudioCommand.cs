using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Studios;

public record UpdateStudioCommand(UpdateStudioInputModel Model) : IRequest<Result<StudioDetailDto>>;

public class UpdateStudioCommandValidator : AbstractValidator<UpdateStudioCommand>
{
    public UpdateStudioCommandValidator()
    {
        RuleFor(x => x.Model.Id)
            .NotEmpty().WithMessage("Id is required");
            
        RuleFor(x => x.Model.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
            
        RuleFor(x => x.Model.Details)
            .MaximumLength(500).WithMessage("Details must not exceed 500 characters");
            
        RuleFor(x => x.Model.ContactInfo)
            .MaximumLength(500).WithMessage("Contact info must not exceed 500 characters");
    }
}

public class UpdateStudioCommandHandler : IRequestHandler<UpdateStudioCommand, Result<StudioDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public UpdateStudioCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<StudioDetailDto>> Handle(UpdateStudioCommand request, CancellationToken cancellationToken)
    {
        var studio = await _context.Studios
            .FirstOrDefaultAsync(s => s.Id == request.Model.Id && s.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (studio == null)
        {
            return Result<StudioDetailDto>.Failure("Studio not found");
        }

        studio.Name = request.Model.Name;
        studio.Details = request.Model.Details;
        studio.ContactInfo = request.Model.ContactInfo;
        studio.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<StudioDetailDto>.Success(new StudioDetailDto
        {
            Id = studio.Id,
            Name = studio.Name,
            Details = studio.Details,
            ContactInfo = studio.ContactInfo
        });
    }
}