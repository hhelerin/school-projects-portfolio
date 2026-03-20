using App.BLL.Features.Students;
using App.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using App.DTO.v1.Students;
using App.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WebApp.Tests.Unit;

public class UpdateStudentCommandTests
{
    private readonly AppDbContext _context;
    private readonly Guid _companyId;
    private readonly TestTenantContext _tenantContext;

    public UpdateStudentCommandTests()
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
    public async Task Handle_ValidInput_UpdatesStudent()
    {
        // Arrange - create existing student
        var student = new Student
        {
            Id = Guid.NewGuid(),
            CompanyId = _companyId,
            Name = "John Doe",
            ContactInfo = "john@example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        var input = new UpdateStudentInputModel
        {
            StudentId = student.Id,
            Name = "John Updated",
            ContactInfo = "john.updated@example.com",
            PersonalId = "12345"
        };

        var handler = new UpdateStudentCommandHandler(_context, _tenantContext);
        var command = new UpdateStudentCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("John Updated", result.Value?.Name);
        Assert.Equal("john.updated@example.com", result.Value?.ContactInfo);
    }

    [Fact]
    public async Task Handle_StudentNotFound_ReturnsFailure()
    {
        // Arrange
        var input = new UpdateStudentInputModel
        {
            StudentId = Guid.NewGuid(),  // Non-existent ID
            Name = "John Doe",
            ContactInfo = "john@example.com"
        };

        var handler = new UpdateStudentCommandHandler(_context, _tenantContext);
        var command = new UpdateStudentCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error?.ToLower());
    }

    [Fact]
    public async Task Handle_InvalidStudentId_ReturnsFailure()
    {
        // Arrange
        var input = new UpdateStudentInputModel
        {
            StudentId = Guid.Empty,  // Empty ID should fail validation
            Name = "John Doe",
            ContactInfo = "john@example.com"
        };

        var validator = new UpdateStudentCommandValidator();
        var validationResult = await validator.ValidateAsync(new UpdateStudentCommand(input));

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == "Model.StudentId");
    }

    [Fact]
    public async Task Handle_EmptyName_ReturnsFailure()
    {
        // Arrange
        var student = new Student
        {
            Id = Guid.NewGuid(),
            CompanyId = _companyId,
            Name = "John Doe",
            ContactInfo = "john@example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        var input = new UpdateStudentInputModel
        {
            StudentId = student.Id,
            Name = "",  // Empty name should fail validation
            ContactInfo = "john@example.com"
        };

        var validator = new UpdateStudentCommandValidator();
        var validationResult = await validator.ValidateAsync(new UpdateStudentCommand(input));

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == "Model.Name");
    }

    [Fact]
    public async Task Handle_NameTooLong_ReturnsFailure()
    {
        // Arrange
        var student = new Student
        {
            Id = Guid.NewGuid(),
            CompanyId = _companyId,
            Name = "John Doe",
            ContactInfo = "john@example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        var input = new UpdateStudentInputModel
        {
            StudentId = student.Id,
            Name = new string('A', 101),  // Exceeds 100 char limit
            ContactInfo = "john@example.com"
        };

        var validator = new UpdateStudentCommandValidator();
        var validationResult = await validator.ValidateAsync(new UpdateStudentCommand(input));

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == "Model.Name");
    }
}
