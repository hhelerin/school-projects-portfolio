using App.DAL.EF;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Levels;

public record DeleteLevelCommand(Guid Id) : IRequest<Result>;

public class DeleteLevelCommandHandler : IRequestHandler<DeleteLevelCommand, Result>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public DeleteLevelCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result> Handle(DeleteLevelCommand request, CancellationToken cancellationToken)
    {
        var level = await _context.Levels
            .FirstOrDefaultAsync(l => l.Id == request.Id && l.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (level == null)
        {
            return Result.Failure("Level not found");
        }

        level.IsDeleted = true;
        level.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}