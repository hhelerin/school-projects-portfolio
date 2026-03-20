using App.DAL.EF;
using App.Domain;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.StudioRooms;

public record AddStudioFeatureCommand(AddStudioFeatureInputModel Model) : IRequest<Result<StudioFeatureDto>>;

public class AddStudioFeatureCommandValidator : AbstractValidator<AddStudioFeatureCommand>
{
    public AddStudioFeatureCommandValidator()
    {
        RuleFor(x => x.Model.StudioRoomId)
            .NotEmpty().WithMessage("Room is required");
            
        RuleFor(x => x.Model.FeatureId)
            .NotEmpty().WithMessage("Feature is required");
    }
}

public class AddStudioFeatureCommandHandler : IRequestHandler<AddStudioFeatureCommand, Result<StudioFeatureDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public AddStudioFeatureCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<StudioFeatureDto>> Handle(AddStudioFeatureCommand request, CancellationToken cancellationToken)
    {
        // Verify room belongs to tenant
        var roomExists = await _context.StudioRooms
            .AnyAsync(r => r.Id == request.Model.StudioRoomId && r.Studio.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (!roomExists)
        {
            return Result<StudioFeatureDto>.Failure("Room not found");
        }

        // Verify feature exists
        var feature = await _context.Features
            .FirstOrDefaultAsync(f => f.Id == request.Model.FeatureId, cancellationToken);

        if (feature == null)
        {
            return Result<StudioFeatureDto>.Failure("Feature not found");
        }

        var studioFeature = new StudioFeature
        {
            StudioRoomId = request.Model.StudioRoomId,
            FeatureId = request.Model.FeatureId,
            ValidFrom = request.Model.ValidFrom,
            ValidUntil = request.Model.ValidUntil
        };

        _context.StudioFeatures.Add(studioFeature);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<StudioFeatureDto>.Success(new StudioFeatureDto
        {
            Id = studioFeature.Id,
            FeatureId = studioFeature.FeatureId,
            FeatureName = feature.Name,
            ValidFrom = studioFeature.ValidFrom,
            ValidUntil = studioFeature.ValidUntil
        });
    }
}