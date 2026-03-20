namespace App.Domain;

public class Feature : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Details { get; set; }
}