using App.BLL.Features.Students;
using App.DAL.EF;
using App.Domain;
using App.DTO.v1.Students;
using App.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WebApp.Tests.Unit;

public class RegisterStudentCommandTests
{
    private readonly AppDbContext _context;
    private readonly Guid _companyId;
    private readonly TestTenantContext _tenantContext;

    public RegisterStudentCommandTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _companyId = Guid.NewGuid();
        _tenantContext = new TestTenantContext(_companyId);
        
        // Seed company
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
        
        _context.Companies.Add(company);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ValidInput_CreatesStudent()
    {
        // Arrange
        var input = new RegisterStudentInputModel
        {
            Name = "John Doe",
            PersonalId = "123456789",
            ContactInfo = "john@example.com",
            Details = "Test student"
        };

        var handler = new RegisterStudentCommandHandler(_context, _tenantContext);
        var command = new RegisterStudentCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        
        // Verify student was created in database
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == result.Value);
        Assert.NotNull(student);
        Assert.Equal("John Doe", student.Name);
    }

    [Fact]
    public async Task Handle_DuplicateNameAndContactInfo_ReturnsWarning()
    {
        // Arrange - create existing student
        var existingStudent = new Student
        {
            Id = Guid.NewGuid(),
            CompanyId = _companyId,
            Name = "Jane Doe",
            ContactInfo = "jane@example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Students.AddAsync(existingStudent);
        await _context.SaveChangesAsync();

        // Try to create duplicate
        var input = new RegisterStudentInputModel
        {
            Name = "Jane Doe",
            ContactInfo = "jane@example.com"
        };

        var handler = new RegisterStudentCommandHandler(_context, _tenantContext);
        var command = new RegisterStudentCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error);
    }

    [Fact]
    public async Task Handle_ConfirmDuplicate_CreatesStudent()
    {
        // Arrange - create existing student
        var existingStudent = new Student
        {
            Id = Guid.NewGuid(),
            CompanyId = _companyId,
            Name = "Jane Doe",
            ContactInfo = "jane@example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Students.AddAsync(existingStudent);
        await _context.SaveChangesAsync();

        // Create with ConfirmDuplicate = true
        var input = new RegisterStudentInputModel
        {
            Name = "Jane Doe",
            ContactInfo = "jane@example.com",
            ConfirmDuplicate = true
        };

        var handler = new RegisterStudentCommandHandler(_context, _tenantContext);
        var command = new RegisterStudentCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_InvalidName_ReturnsFailure()
    {
        // Arrange
        var input = new RegisterStudentInputModel
        {
            Name = "",  // Empty name should fail validation
            ContactInfo = "test@example.com"
        };

        var validator = new RegisterStudentCommandValidator();
        var validationResult = await validator.ValidateAsync(new RegisterStudentCommand(input));

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == "Model.Name");
    }

    [Fact]
    public async Task Handle_NameTooLong_ReturnsFailure()
    {
        // Arrange
        var input = new RegisterStudentInputModel
        {
            Name = new string('A', 101),  // Exceeds 100 char limit
            ContactInfo = "test@example.com"
        };

        var validator = new RegisterStudentCommandValidator();
        var validationResult = await validator.ValidateAsync(new RegisterStudentCommand(input));

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == "Model.Name");
    }
}
