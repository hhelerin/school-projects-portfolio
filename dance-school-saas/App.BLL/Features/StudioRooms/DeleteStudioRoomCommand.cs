using App.DAL.EF;
using App.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.StudioRooms;

public record DeleteStudioRoomCommand(Guid Id) : IRequest<Result>;

public class DeleteStudioRoomCommandHandler : IRequestHandler<DeleteStudioRoomCommand, Result>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public DeleteStudioRoomCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result> Handle(DeleteStudioRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _context.StudioRooms
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.Studio.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (room == null)
        {
            return Result.Failure("Room not found");
        }

        room.IsDeleted = true;
        room.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}