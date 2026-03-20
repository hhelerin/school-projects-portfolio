using App.DAL.EF;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.StudioRooms;

public record UpdateStudioRoomCommand(UpdateStudioRoomInputModel Model) : IRequest<Result<StudioRoomDetailDto>>;

public class UpdateStudioRoomCommandValidator : AbstractValidator<UpdateStudioRoomCommand>
{
    public UpdateStudioRoomCommandValidator()
    {
        RuleFor(x => x.Model.Id)
            .NotEmpty().WithMessage("Id is required");
            
        RuleFor(x => x.Model.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
            
        RuleFor(x => x.Model.Details)
            .MaximumLength(500).WithMessage("Details must not exceed 500 characters");
    }
}

public class UpdateStudioRoomCommandHandler : IRequestHandler<UpdateStudioRoomCommand, Result<StudioRoomDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public UpdateStudioRoomCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<StudioRoomDetailDto>> Handle(UpdateStudioRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _context.StudioRooms
            .FirstOrDefaultAsync(r => r.Id == request.Model.Id && r.Studio.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (room == null)
        {
            return Result<StudioRoomDetailDto>.Failure("Room not found");
        }

        room.Name = request.Model.Name;
        room.Details = request.Model.Details;
        room.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<StudioRoomDetailDto>.Success(new StudioRoomDetailDto
        {
            Id = room.Id,
            StudioId = room.StudioId,
            Name = room.Name,
            Details = room.Details
        });
    }
}