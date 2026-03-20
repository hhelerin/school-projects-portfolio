namespace App.DTO.v1.SchoolConfiguration;

public class DanceStyleListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class DanceStyleDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
}

public class CreateDanceStyleInputModel
{
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
}

public class UpdateDanceStyleInputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
}