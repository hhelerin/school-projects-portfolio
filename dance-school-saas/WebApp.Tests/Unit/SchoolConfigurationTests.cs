using App.BLL.Features.DanceStyles;
using App.BLL.Features.Instructors;
using App.BLL.Features.StudioRooms;
using App.DAL.EF;
using App.Domain;
using App.DTO.v1.SchoolConfiguration;
using App.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace WebApp.Tests.Unit;

public class SchoolConfigurationTests
{
    private readonly AppDbContext _context;
    private readonly Mock<ITenantContext> _tenantContextMock;

    public SchoolConfigurationTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        
        // Setup tenant context mock
        _tenantContextMock = new Mock<ITenantContext>();
        _tenantContextMock.Setup(t => t.CompanyId).Returns(Guid.NewGuid());
    }

    private void SetupTenant(Guid companyId)
    {
        _tenantContextMock.Setup(t => t.CompanyId).Returns(companyId);
    }

    #region DanceStyle Tests

    [Fact]
    public async Task CreateDanceStyleCommand_ValidInput_CreatesStyleScopedToCorrectTenant()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        SetupTenant(companyId);
        
        var input = new CreateDanceStyleInputModel
        {
            Name = "Ballet",
            Details = "Classical ballet style"
        };

        var handler = new CreateDanceStyleCommandHandler(_context, _tenantContextMock.Object);
        var command = new CreateDanceStyleCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Ballet", result.Value.Name);
        
        // Verify in database
        var style = await _context.DanceStyles.FirstOrDefaultAsync();
        Assert.NotNull(style);
        Assert.Equal(companyId, style.CompanyId);
        Assert.False(style.IsDeleted);
    }

    [Fact]
    public async Task DeleteDanceStyleCommand_ExistingStyle_SetsIsDeletedDoesNotHardDelete()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        SetupTenant(companyId);
        
        var style = new DanceStyle
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = "Jazz",
            Details = "Jazz dance style",
            IsDeleted = false
        };
        await _context.DanceStyles.AddAsync(style);
        await _context.SaveChangesAsync();

        var handler = new DeleteDanceStyleCommandHandler(_context, _tenantContextMock.Object);
        var command = new DeleteDanceStyleCommand(style.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        
        // Verify soft delete - entity still exists but IsDeleted is true
        // Need to ignore query filters to find soft-deleted entities
        var deletedStyle = await _context.DanceStyles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(ds => ds.Id == style.Id);
        Assert.NotNull(deletedStyle);
        Assert.True(deletedStyle.IsDeleted);
    }

    [Fact]
    public async Task GetListDanceStyleQuery_MultipleTenants_OnlyReturnsStylesForCurrentTenant()
    {
        // Arrange
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        
        // Add styles for tenant A
        await _context.DanceStyles.AddRangeAsync(
            new DanceStyle { Id = Guid.NewGuid(), CompanyId = tenantA, Name = "Ballet", IsDeleted = false },
            new DanceStyle { Id = Guid.NewGuid(), CompanyId = tenantA, Name = "Jazz", IsDeleted = false }
        );
        
        // Add styles for tenant B
        await _context.DanceStyles.AddRangeAsync(
            new DanceStyle { Id = Guid.NewGuid(), CompanyId = tenantB, Name = "Hip Hop", IsDeleted = false },
            new DanceStyle { Id = Guid.NewGuid(), CompanyId = tenantB, Name = "Tap", IsDeleted = false }
        );
        
        await _context.SaveChangesAsync();

        SetupTenant(tenantA);
        var handler = new GetListDanceStyleQueryHandler(_context, _tenantContextMock.Object);
        var query = new GetListDanceStyleQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Contains(result.Value, ds => ds.Name == "Ballet");
        Assert.Contains(result.Value, ds => ds.Name == "Jazz");
        Assert.DoesNotContain(result.Value, ds => ds.Name == "Hip Hop");
        Assert.DoesNotContain(result.Value, ds => ds.Name == "Tap");
        
        // Verify tenant isolation - ensure we only got tenant A's data
        var dbStyles = await _context.DanceStyles.Where(ds => ds.CompanyId == tenantA).ToListAsync();
        Assert.Equal(2, dbStyles.Count);
    }

    #endregion

    #region StudioFeature Tests

    [Fact]
    public async Task AddStudioFeatureCommand_ValidInput_CreatesJoinRecordWithCorrectDates()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        SetupTenant(companyId);
        
        // Setup studio and room
        var studio = new Studio
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = "Main Studio"
        };
        await _context.Studios.AddAsync(studio);
        
        var room = new StudioRoom
        {
            Id = Guid.NewGuid(),
            StudioId = studio.Id,
            Name = "Room A"
        };
        await _context.StudioRooms.AddAsync(room);
        
        var feature = new Feature
        {
            Id = Guid.NewGuid(),
            Name = "Sprung Floor"
        };
        await _context.Features.AddAsync(feature);
        await _context.SaveChangesAsync();

        var validFrom = DateTime.UtcNow.Date;
        var validUntil = validFrom.AddYears(1);
        
        var input = new AddStudioFeatureInputModel
        {
            StudioRoomId = room.Id,
            FeatureId = feature.Id,
            ValidFrom = validFrom,
            ValidUntil = validUntil
        };

        var handler = new AddStudioFeatureCommandHandler(_context, _tenantContextMock.Object);
        var command = new AddStudioFeatureCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(feature.Id, result.Value.FeatureId);
        Assert.Equal(validFrom, result.Value.ValidFrom);
        Assert.Equal(validUntil, result.Value.ValidUntil);
        
        // Verify in database
        var studioFeature = await _context.StudioFeatures.FirstOrDefaultAsync();
        Assert.NotNull(studioFeature);
        Assert.Equal(room.Id, studioFeature.StudioRoomId);
        Assert.Equal(feature.Id, studioFeature.FeatureId);
    }

    [Fact]
    public async Task RemoveStudioFeatureCommand_ExistingJoinRecord_RemovesJoinRecord()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        SetupTenant(companyId);
        
        // Setup studio, room, feature and join record
        var studio = new Studio
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = "Main Studio"
        };
        await _context.Studios.AddAsync(studio);
        
        var room = new StudioRoom
        {
            Id = Guid.NewGuid(),
            StudioId = studio.Id,
            Name = "Room A"
        };
        await _context.StudioRooms.AddAsync(room);
        
        var feature = new Feature
        {
            Id = Guid.NewGuid(),
            Name = "Mirrors"
        };
        await _context.Features.AddAsync(feature);
        
        var studioFeature = new StudioFeature
        {
            Id = Guid.NewGuid(),
            StudioRoomId = room.Id,
            FeatureId = feature.Id
        };
        await _context.StudioFeatures.AddAsync(studioFeature);
        await _context.SaveChangesAsync();

        var handler = new RemoveStudioFeatureCommandHandler(_context, _tenantContextMock.Object);
        var command = new RemoveStudioFeatureCommand(studioFeature.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        
        // Verify the join record was deleted (hard delete for StudioFeature)
        var deletedRecord = await _context.StudioFeatures.FirstOrDefaultAsync(sf => sf.Id == studioFeature.Id);
        Assert.Null(deletedRecord);
    }

    #endregion

    #region Instructor Tests

    [Fact]
    public async Task CreateInstructorCommand_ValidInput_CreatesInstructorUnderTenant()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        SetupTenant(companyId);
        
        var input = new CreateInstructorInputModel
        {
            Name = "Jane Doe",
            PersonalId = "ID123456",
            ContactInfo = "jane@example.com",
            Details = "Senior ballet instructor"
        };

        var handler = new CreateInstructorCommandHandler(_context, _tenantContextMock.Object);
        var command = new CreateInstructorCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Jane Doe", result.Value.Name);
        Assert.Equal("ID123456", result.Value.PersonalId);
        
        // Verify in database
        var instructor = await _context.Instructors.FirstOrDefaultAsync();
        Assert.NotNull(instructor);
        Assert.Equal(companyId, instructor.CompanyId);
        Assert.False(instructor.IsDeleted);
        Assert.Equal(companyId, instructor.CompanyId);
        Assert.Equal("Jane Doe", instructor.Name);
    }

    [Fact]
    public async Task CreateInstructorCommand_WithAppUserId_CreatesInstructorWithLink()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var appUserId = Guid.NewGuid();
        SetupTenant(companyId);
        
        // Create a CompanyUser record to link the AppUser to the company
        var companyUser = new CompanyUser
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            AppUserId = appUserId,
            IsActive = true,
            JoinedAt = DateTime.UtcNow
        };
        await _context.CompanyUsers.AddAsync(companyUser);
        await _context.SaveChangesAsync();
        
        var input = new CreateInstructorInputModel
        {
            Name = "John Smith",
            PersonalId = "ID789012",
            ContactInfo = "john@example.com",
            AppUserId = appUserId,
            Details = "Jazz instructor"
        };

        var handler = new CreateInstructorCommandHandler(_context, _tenantContextMock.Object);
        var command = new CreateInstructorCommand(input);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(appUserId, result.Value.AppUserId);
        
        // Verify in database
        var instructor = await _context.Instructors.FirstOrDefaultAsync();
        Assert.NotNull(instructor);
        Assert.Equal(appUserId, instructor.AppUserId);
    }

    #endregion
}