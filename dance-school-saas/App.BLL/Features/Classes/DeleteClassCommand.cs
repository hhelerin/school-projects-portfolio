using App.DAL.EF;
using App.Domain;
using App.Helpers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Features.Classes;

public record DeleteClassCommand(Guid ClassId) : IRequest<Result>;

public class DeleteClassCommandValidator : AbstractValidator<DeleteClassCommand>
{
    public DeleteClassCommandValidator()
    {
        RuleFor(x => x.ClassId)
            .NotEmpty().WithMessage("Class ID is required");
    }
}

public class DeleteClassCommandHandler : IRequestHandler<DeleteClassCommand, Result>
{
    private readonly AppDbContext _context;
    private readonly ITenantContext _tenantContext;

    public DeleteClassCommandHandler(AppDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Result> Handle(DeleteClassCommand request, CancellationToken cancellationToken)
    {
        var classEntity = await _context.Classes
            .FirstOrDefaultAsync(c => c.Id == request.ClassId && c.CompanyId == _tenantContext.CompanyId, cancellationToken);
            
        if (classEntity == null)
            return Result.Failure("Class not found");

        var today = DateOnly.FromDateTime(DateTime.Today);
        
        // Soft delete future occurrences that are not already cancelled
        var futureSchedules = await _context.ClassSchedules
            .Where(cs => cs.ClassId == request.ClassId && cs.Date >= today && !cs.IsCancelled && !cs.IsDeleted)
            .ToListAsync(cancellationToken);
            
        foreach (var schedule in futureSchedules)
        {
            schedule.IsDeleted = true;
        }

        // Soft delete the class
        classEntity.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
