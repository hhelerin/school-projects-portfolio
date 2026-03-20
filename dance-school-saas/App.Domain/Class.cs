namespace App.Domain;

public class Class : BaseEntity, ITenantEntity
{
    public Guid CompanyId { get; set; }
    
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
    
    public Guid StudioRoomId { get; set; }
    public StudioRoom StudioRoom { get; set; } = default!;
    
    public Guid InstructorId { get; set; }
    public Instructor Instructor { get; set; } = default!;
    
    public Guid DanceStyleId { get; set; }
    public DanceStyle DanceStyle { get; set; } = default!;
    
    public Guid LevelId { get; set; }
    public Level Level { get; set; } = default!;
    
    public int Capacity { get; set; }
    
    public RecurrencePattern Recurrence { get; set; } = default!;
    
    public ICollection<ClassSchedule> Schedules { get; set; } = new List<ClassSchedule>();
}

public class RecurrencePattern
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateOnly RecurrenceStartDate { get; set; }
    public DateOnly RecurrenceEndDate { get; set; }
}
