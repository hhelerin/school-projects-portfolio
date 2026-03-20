using App.DAL.EF;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.StudioRooms;

public record RemoveStudioFeatureCommand(Guid StudioFeatureId) : IRequest<Result>;

public class RemoveStudioFeatureCommandHandler : IRequestHandler<RemoveStudioFeatureCommand, Result>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public RemoveStudioFeatureCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result> Handle(RemoveStudioFeatureCommand request, CancellationToken cancellationToken)
    {
        var studioFeature = await _context.StudioFeatures
            .FirstOrDefaultAsync(sf => sf.Id == request.StudioFeatureId && sf.StudioRoom.Studio.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (studioFeature == null)
        {
            return Result.Failure("Feature association not found");
        }

        _context.StudioFeatures.Remove(studioFeature);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}