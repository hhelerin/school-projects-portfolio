using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Levels;

public record GetLevelByIdQuery(Guid Id) : IRequest<Result<LevelDetailDto>>;

public class GetLevelByIdQueryHandler : IRequestHandler<GetLevelByIdQuery, Result<LevelDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public GetLevelByIdQueryHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<LevelDetailDto>> Handle(GetLevelByIdQuery request, CancellationToken cancellationToken)
    {
        var level = await _context.Levels
            .Where(l => l.Id == request.Id && l.CompanyId == _tenantContext.CompanyId)
            .Select(l => new LevelDetailDto
            {
                Id = l.Id,
                Name = l.Name,
                Details = l.Details
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (level == null)
        {
            return Result<LevelDetailDto>.Failure("Level not found");
        }

        return Result<LevelDetailDto>.Success(level);
    }
}