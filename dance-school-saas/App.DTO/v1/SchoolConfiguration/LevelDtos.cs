namespace App.DTO.v1.SchoolConfiguration;

public class LevelListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class LevelDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
}

public class CreateLevelInputModel
{
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
}

public class UpdateLevelInputModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
}