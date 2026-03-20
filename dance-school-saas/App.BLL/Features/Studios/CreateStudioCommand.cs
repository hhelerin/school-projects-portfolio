using App.DAL.EF;
using App.Domain;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using FluentValidation;
using MediatR;

namespace App.BLL.Features.Studios;

public record CreateStudioCommand(CreateStudioInputModel Model) : IRequest<Result<StudioDetailDto>>;

public class CreateStudioCommandValidator : AbstractValidator<CreateStudioCommand>
{
    public CreateStudioCommandValidator()
    {
        RuleFor(x => x.Model.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
            
        RuleFor(x => x.Model.Details)
            .MaximumLength(500).WithMessage("Details must not exceed 500 characters");
            
        RuleFor(x => x.Model.ContactInfo)
            .MaximumLength(500).WithMessage("Contact info must not exceed 500 characters");
    }
}

public class CreateStudioCommandHandler : IRequestHandler<CreateStudioCommand, Result<StudioDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateStudioCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<StudioDetailDto>> Handle(CreateStudioCommand request, CancellationToken cancellationToken)
    {
        var studio = new Studio
        {
            CompanyId = _tenantContext.CompanyId,
            Name = request.Model.Name,
            Details = request.Model.Details,
            ContactInfo = request.Model.ContactInfo
        };

        _context.Studios.Add(studio);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<StudioDetailDto>.Success(new StudioDetailDto
        {
            Id = studio.Id,
            Name = studio.Name,
            Details = studio.Details,
            ContactInfo = studio.ContactInfo,
            Rooms = new List<StudioRoomListItemDto>()
        });
    }
}