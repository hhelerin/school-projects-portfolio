using App.DAL.EF;
using App.Domain;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.StudioRooms;

public record CreateStudioRoomCommand(CreateStudioRoomInputModel Model) : IRequest<Result<StudioRoomDetailDto>>;

public class CreateStudioRoomCommandValidator : AbstractValidator<CreateStudioRoomCommand>
{
    public CreateStudioRoomCommandValidator()
    {
        RuleFor(x => x.Model.StudioId)
            .NotEmpty().WithMessage("Studio is required");
            
        RuleFor(x => x.Model.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
            
        RuleFor(x => x.Model.Details)
            .MaximumLength(500).WithMessage("Details must not exceed 500 characters");
    }
}

public class CreateStudioRoomCommandHandler : IRequestHandler<CreateStudioRoomCommand, Result<StudioRoomDetailDto>>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateStudioRoomCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result<StudioRoomDetailDto>> Handle(CreateStudioRoomCommand request, CancellationToken cancellationToken)
    {
        // Verify studio belongs to tenant
        var studioExists = await _context.Studios
            .AnyAsync(s => s.Id == request.Model.StudioId && s.CompanyId == _tenantContext.CompanyId, cancellationToken);

        if (!studioExists)
        {
            return Result<StudioRoomDetailDto>.Failure("Studio not found");
        }

        var room = new StudioRoom
        {
            StudioId = request.Model.StudioId,
            Name = request.Model.Name,
            Details = request.Model.Details
        };

        _context.StudioRooms.Add(room);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<StudioRoomDetailDto>.Success(new StudioRoomDetailDto
        {
            Id = room.Id,
            StudioId = room.StudioId,
            Name = room.Name,
            Details = room.Details,
            Features = new List<StudioFeatureDto>()
        });
    }
}