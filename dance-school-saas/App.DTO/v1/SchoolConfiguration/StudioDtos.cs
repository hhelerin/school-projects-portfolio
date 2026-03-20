namespace App.DTO.v1.SchoolConfiguration;

public class StudioListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int RoomCount { get; set; }
}

public class StudioDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
    public string? ContactInfo { get; set; }
    public int RoomCount => Rooms?.Count ?? 0;
    public List<StudioRoomListItemDto> Rooms { get; set; } = new();
}

public class CreateStudioInputModel
{
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
    public string? ContactInfo { get; set; }
}

public class UpdateStudioInputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
    public string? ContactInfo { get; set; }
}