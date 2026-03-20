using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.DanceStyles;

public record UpdateDanceStyleCommand(UpdateDanceStyleInputModel Model) : IRequest<Result<DanceStyleDetailDto>>;

public class UpdateDanceStyleCommandValidator : AbstractValidator<UpdateDanceStyleCommand>
{
    public UpdateDanceStyleCommandValidator()
    {
        RuleFor(x => x.Model.Id)
            .NotEmpty().WithMessage("Id is required");
            
        RuleFor(x => x.Model.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
            
        RuleFor(x => x.Model.Details)
            .MaximumLength(500).WithMessage("Details must not exceed 500 characters");
    }
}

public class UpdateDanceStyleCommandHandler : IRequestHandler<UpdateDanceStyleCommand, Result<DanceStyleDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public UpdateDanceStyleCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<DanceStyleDetailDto>> Handle(UpdateDanceStyleCommand request, CancellationToken cancellationToken)
    {
        var style = await _context.DanceStyles
            .FirstOrDefaultAsync(ds => ds.Id == request.Model.Id && ds.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (style == null)
        {
            return Result<DanceStyleDetailDto>.Failure("Dance style not found");
        }

        style.Name = request.Model.Name;
        style.Details = request.Model.Details;
        style.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<DanceStyleDetailDto>.Success(new DanceStyleDetailDto
        {
            Id = style.Id,
            Name = style.Name,
            Details = style.Details
        });
    }
}