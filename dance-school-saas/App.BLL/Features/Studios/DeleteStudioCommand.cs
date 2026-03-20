using App.DAL.EF;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Studios;

public record DeleteStudioCommand(Guid Id) : IRequest<Result>;

public class DeleteStudioCommandHandler : IRequestHandler<DeleteStudioCommand, Result>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public DeleteStudioCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result> Handle(DeleteStudioCommand request, CancellationToken cancellationToken)
    {
        var studio = await _context.Studios
            .Include(s => s.Rooms)
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (studio == null)
        {
            return Result.Failure("Studio not found");
        }

        if (studio.Rooms.Any(r => !r.IsDeleted))
        {
            return Result.Failure("Cannot delete studio that has rooms. Please delete all rooms first.");
        }

        studio.IsDeleted = true;
        studio.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}