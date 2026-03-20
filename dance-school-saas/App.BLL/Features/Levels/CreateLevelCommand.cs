using App.DAL.EF;
using App.Domain;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using FluentValidation;
using MediatR;

namespace App.BLL.Features.Levels;

public record CreateLevelCommand(CreateLevelInputModel Model) : IRequest<Result<LevelDetailDto>>;

public class CreateLevelCommandValidator : AbstractValidator<CreateLevelCommand>
{
    public CreateLevelCommandValidator()
    {
        RuleFor(x => x.Model.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
            
        RuleFor(x => x.Model.Details)
            .MaximumLength(500).WithMessage("Details must not exceed 500 characters");
    }
}

public class CreateLevelCommandHandler : IRequestHandler<CreateLevelCommand, Result<LevelDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateLevelCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<LevelDetailDto>> Handle(CreateLevelCommand request, CancellationToken cancellationToken)
    {
        var level = new Level
        {
            CompanyId = _tenantContext.CompanyId,
            Name = request.Model.Name,
            Details = request.Model.Details
        };

        _context.Levels.Add(level);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<LevelDetailDto>.Success(new LevelDetailDto
        {
            Id = level.Id,
            Name = level.Name,
            Details = level.Details
        });
    }
}