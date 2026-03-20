using App.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using App.DTO.v1.Identity;
using App.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Commands;

public record SignUpSchoolCommand(SignUpSchoolInputModel Model) : IRequest<Result<SignUpSchoolResult>>;

public class SignUpSchoolCommandHandler : IRequestHandler<SignUpSchoolCommand, Result<SignUpSchoolResult>>
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public SignUpSchoolCommandHandler(AppDbContext context, UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
    {
        _context = context;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SignUpSchoolResult>> Handle(SignUpSchoolCommand request, CancellationToken cancellationToken)
    {
        var model = request.Model;
        
        // Normalize slug to lowercase
        var normalizedSlug = model.Slug.ToLower();

        // Check slug uniqueness (case-insensitive)
        var slugExists = await _context.Companies
            .AnyAsync(c => c.Slug.ToLower() == normalizedSlug, cancellationToken);
        
        if (slugExists)
        {
            return Result<SignUpSchoolResult>.Failure("A school with this URL already exists");
        }

        // Check email uniqueness
        var emailExists = await _userManager.FindByEmailAsync(model.Email);
        if (emailExists != null)
        {
            return Result<SignUpSchoolResult>.Failure("An account with this email already exists");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // 1. Create Company
            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = model.SchoolName,
                Slug = normalizedSlug,
                IsActive = true,
                SubscriptionTier = SubscriptionTier.Free,
                RegistrationCode = model.RegistrationCode,
                VATCode = model.VATCode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _context.Companies.AddAsync(company, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            // 2. Create AppUser for owner
            var appUser = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.OwnerFirstName,
                LastName = model.OwnerLastName,
                PreferredCompanyId = company.Id,
                EmailConfirmed = true // Auto-confirm for streamlined onboarding
            };

            var createUserResult = await _userManager.CreateAsync(appUser, model.Password);
            if (!createUserResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync();
                var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                return Result<SignUpSchoolResult>.Failure($"Failed to create user account: {errors}");
            }

            // 3. Create CompanyUser
            var companyUser = new CompanyUser
            {
                Id = Guid.NewGuid(),
                AppUserId = appUser.Id,
                CompanyId = company.Id,
                IsActive = true,
                JoinedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _context.CompanyUsers.AddAsync(companyUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            // 4. Assign CompanyOwner role
            var companyOwnerRole = await _context.CompanyRoles
                .FirstOrDefaultAsync(r => r.Name == "CompanyOwner" && r.CompanyId == company.Id, cancellationToken);
            
            if (companyOwnerRole == null)
            {
                // If role doesn't exist for this company, create it
                companyOwnerRole = new CompanyRole
                {
                    Id = Guid.NewGuid(),
                    Name = "CompanyOwner",
                    CompanyId = company.Id,
                    IsDefault = false,
                    IsSystemProtected = true,
                    SortOrder = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.CompanyRoles.AddAsync(companyOwnerRole, cancellationToken);
                await _unitOfWork.SaveChangesAsync();
            }

            var companyUserRole = new CompanyUserRole
            {
                Id = Guid.NewGuid(),
                CompanyUserId = companyUser.Id,
                CompanyRoleId = companyOwnerRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _context.AddAsync(companyUserRole, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            // Commit transaction
            await _unitOfWork.CommitTransactionAsync();

            return Result<SignUpSchoolResult>.Success(new SignUpSchoolResult
            {
                CompanyId = company.Id,
                Slug = company.Slug
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Result<SignUpSchoolResult>.Failure($"An error occurred during registration: {ex.Message}");
        }
    }
}
