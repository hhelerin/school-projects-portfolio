namespace App.DTO.v1.ClassScheduling;

public class CreateClassInputModel
{
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
    public Guid StudioRoomId { get; set; }
    public Guid InstructorId { get; set; }
    public Guid DanceStyleId { get; set; }
    public Guid LevelId { get; set; }
    public int Capacity { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateOnly RecurrenceStartDate { get; set; }
    public DateOnly RecurrenceEndDate { get; set; }
}

public class UpdateClassInputModel
{
    public Guid ClassId { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
    public int Capacity { get; set; }
}

public class EditClassScheduleInstanceInputModel
{
    public Guid ClassScheduleId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public Guid StudioRoomId { get; set; }
    public string? Details { get; set; }
}

public class CancelClassScheduleInstanceInputModel
{
    public Guid ClassScheduleId { get; set; }
    public string CancellationReason { get; set; } = default!;
}
