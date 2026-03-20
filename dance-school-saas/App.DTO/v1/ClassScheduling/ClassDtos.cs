namespace App.DTO.v1.ClassScheduling;

public class ClassListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string DanceStyleName { get; set; } = default!;
    public string LevelName { get; set; } = default!;
    public string InstructorName { get; set; } = default!;
    public string StudioRoomName { get; set; } = default!;
    public int Capacity { get; set; }
    public string RecurrenceSummary { get; set; } = default!;
}

public class ClassDetailDto : ClassListItemDto
{
    public string? Details { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateOnly RecurrenceStartDate { get; set; }
    public DateOnly RecurrenceEndDate { get; set; }
    public int UpcomingScheduleCount { get; set; }
}

public class ClassScheduleListItemDto
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = default!;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string StudioRoomName { get; set; } = default!;
    public string InstructorName { get; set; } = default!;
    public string DanceStyleName { get; set; } = default!;
    public string LevelName { get; set; } = default!;
    public bool IsCancelled { get; set; }
    public bool IsException { get; set; }
    public string? CancellationReason { get; set; }
}
