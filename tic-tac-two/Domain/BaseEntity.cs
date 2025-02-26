using System.ComponentModel.DataAnnotations;

namespace Domain;

public abstract class BaseEntity
{
    [MaxLength(36)]
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
}