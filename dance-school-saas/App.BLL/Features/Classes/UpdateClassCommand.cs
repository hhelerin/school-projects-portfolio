using App.DAL.EF;
using App.Domain;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Classes;

public record UpdateClassCommand(Guid ClassId, string Name, string? Details, int Capacity) : IRequest<Result>;

public class UpdateClassCommandValidator : AbstractValidator<UpdateClassCommand>
{
    public UpdateClassCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");
            
        RuleFor(x => x.Details)
            .MaximumLength(1000).WithMessage("Details must not exceed 1000 characters");
            
        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0");
    }
}

public class UpdateClassCommandHandler : IRequestHandler<UpdateClassCommand, Result>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public UpdateClassCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
    {
        var classEntity = await _context.Classes
            .FirstOrDefaultAsync(c => c.Id == request.ClassId && c.CompanyId == _tenantContext.CompanyId, cancellationToken);
            
        if (classEntity == null)
            return Result.Failure("Class not found");

        classEntity.Name = request.Name;
        classEntity.Details = request.Details;
        classEntity.Capacity = request.Capacity;

        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
