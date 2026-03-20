using App.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using App.DTO.v1.Identity;
using App.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace App.BLL.Commands;

public record InviteStaffCommand(InviteStaffInputModel Model, Guid CompanyId, Guid InvitedByUserId) : IRequest<Result<InviteStaffResult>>;

public class InviteStaffCommandHandler : IRequestHandler<InviteStaffCommand, Result<InviteStaffResult>>
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InviteStaffCommandHandler> _logger;

    public InviteStaffCommandHandler(
        AppDbContext context, 
        UserManager<AppUser> userManager, 
        IUnitOfWork unitOfWork,
        ILogger<InviteStaffCommandHandler> logger)
    {
        _context = context;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<InviteStaffResult>> Handle(InviteStaffCommand request, CancellationToken cancellationToken)
    {
        var model = request.Model;

        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        
        if (existingUser != null)
        {
            // Check if user is already a member of this company
            var existingCompanyUser = await _context.CompanyUsers
                .FirstOrDefaultAsync(cu => cu.AppUserId == existingUser.Id && cu.CompanyId == request.CompanyId, cancellationToken);
            
            if (existingCompanyUser != null)
            {
                return Result<InviteStaffResult>.Failure("This user is already a member of your school");
            }

            await _unitOfWork.BeginTransactionAsync();
            
            try
            {
                // Create CompanyUser for existing user
                var companyUser = new CompanyUser
                {
                    Id = Guid.NewGuid(),
                    AppUserId = existingUser.Id,
                    CompanyId = request.CompanyId,
                    IsActive = true,
                    JoinedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                await _context.CompanyUsers.AddAsync(companyUser, cancellationToken);
                await _unitOfWork.SaveChangesAsync();

                // Assign role
                var roleAssignmentResult = await AssignRoleAsync(companyUser.Id, model.CompanyRoleId, request.CompanyId, cancellationToken);
                if (!roleAssignmentResult.IsSuccess)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<InviteStaffResult>.Failure(roleAssignmentResult.Error!);
                }

                await _unitOfWork.CommitTransactionAsync();

                return Result<InviteStaffResult>.Success(new InviteStaffResult
                {
                    UserId = existingUser.Id
                });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<InviteStaffResult>.Failure($"An error occurred: {ex.Message}");
            }
        }
        else
        {
            // Create new user with temporary password
            await _unitOfWork.BeginTransactionAsync();
            
            try
            {
                // Generate temporary password
                var tempPassword = GenerateTemporaryPassword();
                
                // Create AppUser
                var newUser = new AppUser
                {
                    Id = Guid.NewGuid(),
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = true // Auto-confirm for invited users
                };

                var createResult = await _userManager.CreateAsync(newUser, tempPassword);
                if (!createResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return Result<InviteStaffResult>.Failure($"Failed to create user: {errors}");
                }

                // Log temporary password to console
                _logger.LogInformation("Temporary password for new user {Email}: {Password}", model.Email, tempPassword);

                // Create CompanyUser
                var companyUser = new CompanyUser
                {
                    Id = Guid.NewGuid(),
                    AppUserId = newUser.Id,
                    CompanyId = request.CompanyId,
                    IsActive = true,
                    JoinedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                await _context.CompanyUsers.AddAsync(companyUser, cancellationToken);
                await _unitOfWork.SaveChangesAsync();

                // Assign role
                var roleAssignmentResult = await AssignRoleAsync(companyUser.Id, model.CompanyRoleId, request.CompanyId, cancellationToken);
                if (!roleAssignmentResult.IsSuccess)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result<InviteStaffResult>.Failure(roleAssignmentResult.Error!);
                }

                await _unitOfWork.CommitTransactionAsync();

                return Result<InviteStaffResult>.Success(new InviteStaffResult
                {
                    UserId = newUser.Id
                });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<InviteStaffResult>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }

    private async Task<Result<bool>> AssignRoleAsync(Guid companyUserId, Guid companyRoleId, Guid companyId, CancellationToken cancellationToken)
    {
        // Verify the role exists and belongs to this company
        var roleExists = await _context.CompanyRoles
            .AnyAsync(r => r.Id == companyRoleId && r.CompanyId == companyId, cancellationToken);
        
        if (!roleExists)
        {
            return Result<bool>.Failure("Selected role does not exist or does not belong to your company");
        }

        var companyUserRole = new CompanyUserRole
        {
            Id = Guid.NewGuid(),
            CompanyUserId = companyUserId,
            CompanyRoleId = companyRoleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.AddAsync(companyUserRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    private static string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var result = new char[12];
        var bytes = new byte[12];
        
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        
        for (int i = 0; i < 12; i++)
        {
            result[i] = chars[bytes[i] % chars.Length];
        }
        
        return new string(result);
    }
}
