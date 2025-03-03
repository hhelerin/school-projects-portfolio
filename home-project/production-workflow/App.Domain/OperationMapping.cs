using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class OperationMapping : BaseEntity
{
    public Guid ProcessingStepId { get; set; }
    public ProcessingStep? ProcessingStep { get; set; }
    
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public bool PrerequisitesObtained { get; set; } = true;
    public DateOnly? CompletedAt { get; set; }
    [MaxLength(128)]
    public string? Details { get; set; } = default!;
}