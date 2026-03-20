using App.DAL.EF;
using App.Domain;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using FluentValidation;
using MediatR;

namespace App.BLL.Features.DanceStyles;

public record CreateDanceStyleCommand(CreateDanceStyleInputModel Model) : IRequest<Result<DanceStyleDetailDto>>;

public class CreateDanceStyleCommandValidator : AbstractValidator<CreateDanceStyleCommand>
{
    public CreateDanceStyleCommandValidator()
    {
        RuleFor(x => x.Model.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
            
        RuleFor(x => x.Model.Details)
            .MaximumLength(500).WithMessage("Details must not exceed 500 characters");
    }
}

public class CreateDanceStyleCommandHandler : IRequestHandler<CreateDanceStyleCommand, Result<DanceStyleDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateDanceStyleCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<DanceStyleDetailDto>> Handle(CreateDanceStyleCommand request, CancellationToken cancellationToken)
    {
        var style = new DanceStyle
        {
            CompanyId = _tenantContext.CompanyId,
            Name = request.Model.Name,
            Details = request.Model.Details
        };

        _context.DanceStyles.Add(style);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<DanceStyleDetailDto>.Success(new DanceStyleDetailDto
        {
            Id = style.Id,
            Name = style.Name,
            Details = style.Details
        });
    }
}