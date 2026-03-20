using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.StudioRooms;

public record GetStudioRoomByIdQuery(Guid Id) : IRequest<Result<StudioRoomDetailDto>>;

public class GetStudioRoomByIdQueryHandler : IRequestHandler<GetStudioRoomByIdQuery, Result<StudioRoomDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetStudioRoomByIdQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<StudioRoomDetailDto>> Handle(GetStudioRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var room = await _context.StudioRooms
            .Where(r => r.Id == request.Id && r.Studio.CompanyId == _tenantContext.CompanyId)
            .Select(r => new StudioRoomDetailDto
            {
                Id = r.Id,
                StudioId = r.StudioId,
                Name = r.Name,
                Details = r.Details,
                Features = r.Features.Select(f => new StudioFeatureDto
                {
                    Id = f.Id,
                    FeatureId = f.FeatureId,
                    FeatureName = f.Feature.Name,
                    ValidFrom = f.ValidFrom,
                    ValidUntil = f.ValidUntil
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (room == null)
        {
            return Result<StudioRoomDetailDto>.Failure("Room not found");
        }

        return Result<StudioRoomDetailDto>.Success(room);
    }
}