using App.BLL.Features.Students;
using App.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using App.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WebApp.Tests.Unit;

public class DeleteStudentCommandTests
{
    private readonly AppDbContext _context;
    private readonly Guid _companyId;
    private readonly TestTenantContext _tenantContext;

    public DeleteStudentCommandTests()
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
    public async Task Handle_ValidId_SoftDeletesStudent()
    {
        // Arrange - create existing student
        var student = new Student
        {
            Id = Guid.NewGuid(),
            CompanyId = _companyId,
            Name = "John Doe",
            ContactInfo = "john@example.com",
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        var handler = new DeleteStudentCommandHandler(_context, _tenantContext);
        var command = new DeleteStudentCommand(student.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        
        // Verify student was soft deleted
        var deletedStudent = await _context.Students
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == student.Id && s.CompanyId == _companyId);
        Assert.NotNull(deletedStudent);
        Assert.True(deletedStudent.IsDeleted);
    }

    [Fact]
    public async Task Handle_StudentNotFound_ReturnsFailure()
    {
        // Arrange
        var handler = new DeleteStudentCommandHandler(_context, _tenantContext);
        var command = new DeleteStudentCommand(Guid.NewGuid());  // Non-existent ID

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error?.ToLower());
    }

    [Fact]
    public async Task Handle_AlreadyDeletedStudent_ReturnsFailure()
    {
        // Arrange - create already deleted student
        var student = new Student
        {
            Id = Guid.NewGuid(),
            CompanyId = _companyId,
            Name = "John Doe",
            ContactInfo = "john@example.com",
            IsDeleted = true,  // Already deleted
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        var handler = new DeleteStudentCommandHandler(_context, _tenantContext);
        var command = new DeleteStudentCommand(student.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        // The current implementation returns "not found" since it filters by IsDeleted = false implicitly
        // This is the expected behavior - trying to delete an already deleted student returns not found
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_DifferentCompany_ReturnsFailure()
    {
        // Arrange - create student in different company
        var differentCompanyId = Guid.NewGuid();
        var student = new Student
        {
            Id = Guid.NewGuid(),
            CompanyId = differentCompanyId,  // Different company
            Name = "John Doe",
            ContactInfo = "john@example.com",
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        var handler = new DeleteStudentCommandHandler(_context, _tenantContext);
        var command = new DeleteStudentCommand(student.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error?.ToLower());
    }
}
