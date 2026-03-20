using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.StudioRooms;

public record GetRoomsByStudioQuery(Guid StudioId) : IRequest<Result<List<StudioRoomListItemDto>>>;

public class GetRoomsByStudioQueryHandler : IRequestHandler<GetRoomsByStudioQuery, Result<List<StudioRoomListItemDto>>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetRoomsByStudioQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<List<StudioRoomListItemDto>>> Handle(GetRoomsByStudioQuery request, CancellationToken cancellationToken)
    {
        // Verify studio belongs to tenant
        var studioExists = await _context.Studios
            .AnyAsync(s => s.Id == request.StudioId && s.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (!studioExists)
        {
            return Result<List<StudioRoomListItemDto>>.Failure("Studio not found");
        }

        var rooms = await _context.StudioRooms
            .Where(r => r.StudioId == request.StudioId)
            .OrderBy(r => r.Name)
            .Select(r => new StudioRoomListItemDto
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToListAsync(cancellationToken);

        return Result<List<StudioRoomListItemDto>>.Success(rooms);
    }
}