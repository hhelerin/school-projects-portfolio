using App.BLL.Features.Students;
using App.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using App.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WebApp.Tests.Unit;

public class GetStudentListQueryTests
{
    private readonly AppDbContext _context;
    private readonly Guid _companyId;
    private readonly TestTenantContext _tenantContext;

    public GetStudentListQueryTests()
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
        
        // Seed multiple students for testing
        var students = new List<Student>
        {
            new Student
            {
                Id = Guid.NewGuid(),
                CompanyId = _companyId,
                Name = "Alice Johnson",
                ContactInfo = "alice@example.com",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Student
            {
                Id = Guid.NewGuid(),
                CompanyId = _companyId,
                Name = "Bob Smith",
                ContactInfo = "bob@example.com",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Student
            {
                Id = Guid.NewGuid(),
                CompanyId = _companyId,
                Name = "Charlie Brown",
                ContactInfo = "charlie@example.com",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            // Deleted student - should not appear in results
            new Student
            {
                Id = Guid.NewGuid(),
                CompanyId = _companyId,
                Name = "Deleted Student",
                ContactInfo = "deleted@example.com",
                IsDeleted = true,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-20)
            },
            // Student in different company - should not appear in results
            new Student
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(), // Different company
                Name = "Other Company Student",
                ContactInfo = "other@example.com",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        
        _context.Students.AddRange(students);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllActiveStudents()
    {
        // Arrange
        var query = new GetStudentListQuery(null);
        var handler = new GetStudentListQueryHandler(_context, _tenantContext);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.TotalCount); // Only active students from our company
    }

    [Fact]
    public async Task Handle_WithSearchTerm_FiltersByName()
    {
        // Arrange
        var query = new GetStudentListQuery(SearchTerm: "alice");
        var handler = new GetStudentListQueryHandler(_context, _tenantContext);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Items);
        Assert.Equal("Alice Johnson", result.Value.Items[0].Name);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_FiltersByContactInfo()
    {
        // Arrange
        var query = new GetStudentListQuery(SearchTerm: "bob@example");
        var handler = new GetStudentListQueryHandler(_context, _tenantContext);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Items);
        Assert.Equal("Bob Smith", result.Value.Items[0].Name);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var query = new GetStudentListQuery(null, 1, 2);
        var handler = new GetStudentListQueryHandler(_context, _tenantContext);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(2, result.Value.PageSize);
        Assert.Equal(1, result.Value.PageNumber);
    }

    [Fact]
    public async Task Handle_SecondPage_ReturnsCorrectItems()
    {
        // Arrange
        var query = new GetStudentListQuery(null, 2, 2);
        var handler = new GetStudentListQueryHandler(_context, _tenantContext);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Items); // Only 1 item on page 2
        Assert.Equal(3, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_ResultsAreOrderedByName()
    {
        // Arrange
        var query = new GetStudentListQuery(null);
        var handler = GetStudentListQueryHandler(_context, _tenantContext);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Items.Count);
        
        // Verify ordering (Alice, Bob, Charlie alphabetically)
        Assert.Equal("Alice Johnson", result.Value.Items[0].Name);
        Assert.Equal("Bob Smith", result.Value.Items[1].Name);
        Assert.Equal("Charlie Brown", result.Value.Items[2].Name);
    }

    private GetStudentListQueryHandler GetStudentListQueryHandler(AppDbContext context, TestTenantContext tenantContext)
    {
        return new GetStudentListQueryHandler(context, tenantContext);
    }
}
