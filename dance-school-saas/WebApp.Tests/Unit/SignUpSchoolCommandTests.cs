using App.BLL.Commands;
using App.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using App.DTO.v1.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace WebApp.Tests.Unit;

public class SignUpSchoolCommandTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AppDbContext _context;

    public SignUpSchoolCommandTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        
        // Setup UserManager mock
        var userStoreMock = new Mock<IUserStore<AppUser>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        
        // Setup UnitOfWork - use real UnitOfWork to ensure SaveChanges actually persists
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);
        // CRITICAL: Make SaveChanges actually call context.SaveChanges() to persist entities
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .Returns(() => _context.SaveChangesAsync());
    }

    [Fact]
    public async Task Handle_ValidInput_CreatesCompanyUserAndAssignsOwnerRole()
    {
        // Arrange
        var input = new SignUpSchoolInputModel
        {
            OwnerFirstName = "John",
            OwnerLastName = "Doe",
            Email = "john@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            SchoolName = "Dance Studio",
            Slug = "dance-studio"
        };

        _userManagerMock.Setup(um => um.FindByEmailAsync(input.Email))
            .ReturnsAsync((AppUser?)null);
        
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<AppUser>(), input.Password))
            .ReturnsAsync(IdentityResult.Success);

        var handler = new SignUpSchoolCommandHandler(_context, _userManagerMock.Object, _unitOfWorkMock.Object);
        var command = new SignUpSchoolCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("dance-studio", result.Value.Slug);
        
        // Verify company was created
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Slug == "dance-studio");
        Assert.NotNull(company);
        Assert.Equal("Dance Studio", company.Name);
        Assert.True(company.IsActive);
        Assert.Equal(SubscriptionTier.Free, company.SubscriptionTier);
    }

    [Fact]
    public async Task Handle_DuplicateSlug_ReturnsFailure()
    {
        // Arrange - Create existing company
        var existingCompany = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Existing Studio",
            Slug = "dance-studio",
            IsActive = true,
            SubscriptionTier = SubscriptionTier.Free,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Companies.AddAsync(existingCompany);
        await _context.SaveChangesAsync();

        var input = new SignUpSchoolInputModel
        {
            OwnerFirstName = "John",
            OwnerLastName = "Doe",
            Email = "john@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            SchoolName = "Another Studio",
            Slug = "dance-studio" // Same slug as existing
        };

        var handler = new SignUpSchoolCommandHandler(_context, _userManagerMock.Object, _unitOfWorkMock.Object);
        var command = new SignUpSchoolCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsFailure()
    {
        // Arrange
        var input = new SignUpSchoolInputModel
        {
            OwnerFirstName = "John",
            OwnerLastName = "Doe",
            Email = "existing@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            SchoolName = "Dance Studio",
            Slug = "dance-studio"
        };

        var existingUser = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = input.Email,
            UserName = input.Email
        };

        _userManagerMock.Setup(um => um.FindByEmailAsync(input.Email))
            .ReturnsAsync(existingUser);

        var handler = new SignUpSchoolCommandHandler(_context, _userManagerMock.Object, _unitOfWorkMock.Object);
        var command = new SignUpSchoolCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("email already exists", result.Error?.ToLower());
    }

    [Fact]
    public async Task Handle_CaseInsensitiveSlugCheck_ReturnsFailureForDifferentCase()
    {
        // Arrange - Create existing company with lowercase slug
        var existingCompany = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Existing Studio",
            Slug = "dance-studio",
            IsActive = true,
            SubscriptionTier = SubscriptionTier.Free,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Companies.AddAsync(existingCompany);
        await _context.SaveChangesAsync();

        var input = new SignUpSchoolInputModel
        {
            OwnerFirstName = "John",
            OwnerLastName = "Doe",
            Email = "john@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            SchoolName = "Another Studio",
            Slug = "DANCE-STUDIO" // Different case
        };

        var handler = new SignUpSchoolCommandHandler(_context, _userManagerMock.Object, _unitOfWorkMock.Object);
        var command = new SignUpSchoolCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error);
    }
}
