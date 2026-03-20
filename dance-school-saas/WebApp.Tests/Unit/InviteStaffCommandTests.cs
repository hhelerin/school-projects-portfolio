using App.BLL.Commands;
using App.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using App.DTO.v1.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace WebApp.Tests.Unit;

public class InviteStaffCommandTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<InviteStaffCommandHandler>> _loggerMock;
    private readonly AppDbContext _context;
    private readonly Guid _companyId;
    private readonly Guid _companyRoleId;

    public InviteStaffCommandTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _companyId = Guid.NewGuid();
        _companyRoleId = Guid.NewGuid();
        
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
        
        // Setup Logger mock
        _loggerMock = new Mock<ILogger<InviteStaffCommandHandler>>();
        
        // Seed company and role
        SeedTestData();
    }

    private void SeedTestData()
    {
        var company = new Company
        {
            Id = _companyId,
            Name = "Test Studio",
            Slug = "test-studio",
            IsActive = true,
            SubscriptionTier = SubscriptionTier.Free,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var role = new CompanyRole
        {
            Id = _companyRoleId,
            Name = "CompanyManager",
            CompanyId = _companyId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _context.Companies.Add(company);
        _context.CompanyRoles.Add(role);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ExistingUser_CreatesCompanyUserOnly()
    {
        // Arrange
        var existingUserId = Guid.NewGuid();
        var existingUser = new AppUser
        {
            Id = existingUserId,
            Email = "existing@example.com",
            UserName = "existing@example.com",
            FirstName = "Existing",
            LastName = "User"
        };

        _userManagerMock.Setup(um => um.FindByEmailAsync("existing@example.com"))
            .ReturnsAsync(existingUser);

        var input = new InviteStaffInputModel
        {
            Email = "existing@example.com",
            FirstName = "Existing",
            LastName = "User",
            CompanyRoleId = _companyRoleId
        };

        var handler = new InviteStaffCommandHandler(
            _context, _userManagerMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        var command = new InviteStaffCommand(input, _companyId, Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(existingUserId, result.Value?.UserId);
        
        // Verify CompanyUser was created
        var companyUser = await _context.CompanyUsers
            .FirstOrDefaultAsync(cu => cu.AppUserId == existingUserId && cu.CompanyId == _companyId);
        Assert.NotNull(companyUser);
        Assert.True(companyUser.IsActive);
        
        // Verify no new AppUser was created
        _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NewUser_CreatesUserWithTempPassword()
    {
        // Arrange
        _userManagerMock.Setup(um => um.FindByEmailAsync("newuser@example.com"))
            .ReturnsAsync((AppUser?)null);
        
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var input = new InviteStaffInputModel
        {
            Email = "newuser@example.com",
            FirstName = "New",
            LastName = "User",
            CompanyRoleId = _companyRoleId
        };

        var handler = new InviteStaffCommandHandler(
            _context, _userManagerMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        var command = new InviteStaffCommand(input, _companyId, Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        
        // Verify AppUser was created
        _userManagerMock.Verify(um => um.CreateAsync(
            It.Is<AppUser>(u => u.Email == "newuser@example.com" && u.FirstName == "New"),
            It.IsAny<string>()), Times.Once);
        
        // Verify temp password was logged
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Temporary password")),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UserAlreadyMember_ReturnsFailure()
    {
        // Arrange
        var existingUserId = Guid.NewGuid();
        var existingUser = new AppUser
        {
            Id = existingUserId,
            Email = "member@example.com",
            UserName = "member@example.com",
            FirstName = "Member",
            LastName = "User"
        };

        // Add existing CompanyUser
        var existingCompanyUser = new CompanyUser
        {
            Id = Guid.NewGuid(),
            AppUserId = existingUserId,
            CompanyId = _companyId,
            IsActive = true,
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.CompanyUsers.AddAsync(existingCompanyUser);
        await _context.SaveChangesAsync();

        _userManagerMock.Setup(um => um.FindByEmailAsync("member@example.com"))
            .ReturnsAsync(existingUser);

        var input = new InviteStaffInputModel
        {
            Email = "member@example.com",
            FirstName = "Member",
            LastName = "User",
            CompanyRoleId = _companyRoleId
        };

        var handler = new InviteStaffCommandHandler(
            _context, _userManagerMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        var command = new InviteStaffCommand(input, _companyId, Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("already a member", result.Error?.ToLower());
    }

    [Fact]
    public async Task Handle_InvalidEmailFormat_ReturnsFailure()
    {
        // Arrange
        var input = new InviteStaffInputModel
        {
            Email = "invalid-email",
            FirstName = "Test",
            LastName = "User",
            CompanyRoleId = _companyRoleId
        };

        var validator = new App.BLL.Validators.InviteStaffCommandValidator();
        var validationResult = await validator.ValidateAsync(
            new InviteStaffCommand(input, _companyId, Guid.NewGuid()));

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == "Model.Email");
    }
}
