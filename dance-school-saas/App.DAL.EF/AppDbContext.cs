using App.Domain;
using App.Domain.Identity;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace App.DAL.EF;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, AppRole, Guid>(options), IDataProtectionKeyContext
{

    public DbSet<AppRefreshToken> RefreshTokens { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyUser> CompanyUsers { get; set; }
    public DbSet<CompanyRole> CompanyRoles { get; set; }
    public DbSet<CompanyUserRole> CompanyUserRoles { get; set; }
    
    // School Configuration entities
    public DbSet<DanceStyle> DanceStyles { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<Studio> Studios { get; set; }
    public DbSet<StudioRoom> StudioRooms { get; set; }
    public DbSet<StudioFeature> StudioFeatures { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    
    // Class Scheduling entities
    public DbSet<Class> Classes { get; set; }
    public DbSet<ClassSchedule> ClassSchedules { get; set; }
    
    // Student Management entities
    public DbSet<Student> Students { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply entity configurations
        builder.ApplyConfiguration(new Configurations.DanceStyleConfiguration());
        builder.ApplyConfiguration(new Configurations.LevelConfiguration());
        builder.ApplyConfiguration(new Configurations.FeatureConfiguration());
        builder.ApplyConfiguration(new Configurations.StudioConfiguration());
        builder.ApplyConfiguration(new Configurations.StudioRoomConfiguration());
        builder.ApplyConfiguration(new Configurations.StudioFeatureConfiguration());
        builder.ApplyConfiguration(new Configurations.InstructorConfiguration());
        builder.ApplyConfiguration(new Configurations.ClassConfiguration());
        builder.ApplyConfiguration(new Configurations.ClassScheduleConfiguration());
        builder.ApplyConfiguration(new Configurations.StudentConfiguration());

        // Configure all DateTime properties to use UTC
        ConfigureDateTimeAsUtc(builder);
        
        // Configure DateOnly and TimeOnly for SQLite
        ConfigureDateOnlyTimeOnly(builder);

        // disable cascade delete
        foreach (var relationship in builder.Model
                     .GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
    
    /// <summary>
    /// Configures all DateTime and DateTime? properties to convert to UTC when saving to PostgreSQL.
    /// PostgreSQL's 'timestamp with time zone' type requires UTC values.
    /// </summary>
    private static void ConfigureDateTimeAsUtc(ModelBuilder builder)
    {
        // Value converter for DateTime
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(v, DateTimeKind.Utc)
                : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        // Value converter for DateTime?
        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue
                ? (v.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                    : v.Value.ToUniversalTime())
                : v,
            v => v.HasValue
                ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                : v);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }
    
    /// <summary>
    /// Configures DateOnly and TimeOnly properties to store as TEXT in SQLite.
    /// </summary>
    private static void ConfigureDateOnlyTimeOnly(ModelBuilder builder)
    {
        // Value converter for DateOnly
        var dateOnlyConverter = new ValueConverter<DateOnly, string>(
            v => v.ToString("O"),
            v => DateOnly.Parse(v));
            
        // Value converter for DateOnly?
        var nullableDateOnlyConverter = new ValueConverter<DateOnly?, string?>(
            v => v.HasValue ? v.Value.ToString("O") : null,
            v => v == null ? null : DateOnly.Parse(v));
            
        // Value converter for TimeOnly
        var timeOnlyConverter = new ValueConverter<TimeOnly, string>(
            v => v.ToString("O"),
            v => TimeOnly.Parse(v));
            
        // Value converter for TimeOnly?
        var nullableTimeOnlyConverter = new ValueConverter<TimeOnly?, string?>(
            v => v.HasValue ? v.Value.ToString("O") : null,
            v => v == null ? null : TimeOnly.Parse(v));

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateOnly))
                {
                    property.SetValueConverter(dateOnlyConverter);
                }
                else if (property.ClrType == typeof(DateOnly?))
                {
                    property.SetValueConverter(nullableDateOnlyConverter);
                }
                else if (property.ClrType == typeof(TimeOnly))
                {
                    property.SetValueConverter(timeOnlyConverter);
                }
                else if (property.ClrType == typeof(TimeOnly?))
                {
                    property.SetValueConverter(nullableTimeOnlyConverter);
                }
            }
        }
    }

}
