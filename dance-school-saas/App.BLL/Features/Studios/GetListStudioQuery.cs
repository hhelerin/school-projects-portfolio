using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Studios;

public record GetListStudioQuery : IRequest<Result<List<StudioListItemDto>>>;

public class GetListStudioQueryHandler : IRequestHandler<GetListStudioQuery, Result<List<StudioListItemDto>>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetListStudioQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<List<StudioListItemDto>>> Handle(GetListStudioQuery request, CancellationToken cancellationToken)
    {
        var studios = await _context.Studios
            .Where(s => s.CompanyId == _tenantContext.CompanyId)
            .OrderBy(s => s.Name)
            .Select(s => new StudioListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                RoomCount = s.Rooms.Count(r => !r.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        return Result<List<StudioListItemDto>>.Success(studios);
    }
}