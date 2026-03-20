namespace App.DTO.v1.SchoolConfiguration;

public class StudioRoomListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
}

public class StudioRoomDetailDto
{
    public Guid Id { get; set; }
    public Guid StudioId { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
    public List<StudioFeatureDto> Features { get; set; } = new();
}

public class CreateStudioRoomInputModel
{
    public Guid StudioId { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
}

public class UpdateStudioRoomInputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
}

public class StudioFeatureDto
{
    public Guid Id { get; set; }
    public Guid FeatureId { get; set; }
    public string FeatureName { get; set; } = default!;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}

public class AddStudioFeatureInputModel
{
    public Guid StudioRoomId { get; set; }
    public Guid FeatureId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}