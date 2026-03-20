using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Levels;

public record UpdateLevelCommand(UpdateLevelInputModel Model) : IRequest<Result<LevelDetailDto>>;

public class UpdateLevelCommandValidator : AbstractValidator<UpdateLevelCommand>
{
    public UpdateLevelCommandValidator()
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

public class UpdateLevelCommandHandler : IRequestHandler<UpdateLevelCommand, Result<LevelDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public UpdateLevelCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<LevelDetailDto>> Handle(UpdateLevelCommand request, CancellationToken cancellationToken)
    {
        var level = await _context.Levels
            .FirstOrDefaultAsync(l => l.Id == request.Model.Id && l.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (level == null)
        {
            return Result<LevelDetailDto>.Failure("Level not found");
        }

        level.Name = request.Model.Name;
        level.Details = request.Model.Details;
        level.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<LevelDetailDto>.Success(new LevelDetailDto
        {
            Id = level.Id,
            Name = level.Name,
            Details = level.Details
        });
    }
}