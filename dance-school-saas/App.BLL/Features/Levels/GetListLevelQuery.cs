using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Levels;

public record GetListLevelQuery : IRequest<Result<List<LevelListItemDto>>>;

public class GetListLevelQueryHandler : IRequestHandler<GetListLevelQuery, Result<List<LevelListItemDto>>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetListLevelQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<List<LevelListItemDto>>> Handle(GetListLevelQuery request, CancellationToken cancellationToken)
    {
        var levels = await _context.Levels
            .Where(l => l.CompanyId == _tenantContext.CompanyId)
            .OrderBy(l => l.Name)
            .Select(l => new LevelListItemDto
            {
                Id = l.Id,
                Name = l.Name
            })
            .ToListAsync(cancellationToken);

        return Result<List<LevelListItemDto>>.Success(levels);
    }
}