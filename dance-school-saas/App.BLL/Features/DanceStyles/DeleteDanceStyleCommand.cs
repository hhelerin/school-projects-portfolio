using App.DAL.EF;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.DanceStyles;

public record DeleteDanceStyleCommand(Guid Id) : IRequest<Result>;

public class DeleteDanceStyleCommandHandler : IRequestHandler<DeleteDanceStyleCommand, Result>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public DeleteDanceStyleCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result> Handle(DeleteDanceStyleCommand request, CancellationToken cancellationToken)
    {
        var style = await _context.DanceStyles
            .FirstOrDefaultAsync(ds => ds.Id == request.Id && ds.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (style == null)
        {
            return Result.Failure("Dance style not found");
        }

        style.IsDeleted = true;
        style.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}