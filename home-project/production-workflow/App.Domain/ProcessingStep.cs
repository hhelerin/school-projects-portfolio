using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class ProcessingStep : BaseEntity
{
    [MaxLength(128)]
    public string Name { get; set; } = default!;    
    [MaxLength(128)]
    public string? Details { get; set; } = default!;
}