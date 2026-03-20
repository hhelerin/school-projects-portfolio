using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.DanceStyles;

public record GetListDanceStyleQuery : IRequest<Result<List<DanceStyleListItemDto>>>;

public class GetListDanceStyleQueryHandler : IRequestHandler<GetListDanceStyleQuery, Result<List<DanceStyleListItemDto>>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetListDanceStyleQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<List<DanceStyleListItemDto>>> Handle(GetListDanceStyleQuery request, CancellationToken cancellationToken)
    {
        var styles = await _context.DanceStyles
            .Where(ds => ds.CompanyId == _tenantContext.CompanyId)
            .OrderBy(ds => ds.Name)
            .Select(ds => new DanceStyleListItemDto
            {
                Id = ds.Id,
                Name = ds.Name
            })
            .ToListAsync(cancellationToken);

        return Result<List<DanceStyleListItemDto>>.Success(styles);
    }
}