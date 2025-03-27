using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class OperationMapping : BaseEntity
{    
    [Display(Name = nameof(ProcessingStep), Prompt = nameof(ProcessingStep), ResourceType = typeof(App.Resources.Domain.OperationMapping))]
    public Guid ProcessingStepId { get; set; }
    [Display(Name = nameof(ProcessingStep), Prompt = nameof(ProcessingStep), ResourceType = typeof(App.Resources.Domain.OperationMapping))]
    public ProcessingStep? ProcessingStep { get; set; }
    
    [Display(Name = nameof(Order), Prompt = nameof(Order), ResourceType = typeof(Base.Resources.Common))]
    public Guid OrderId { get; set; }
    [Display(Name = nameof(Order), Prompt = nameof(Order), ResourceType = typeof(Base.Resources.Common))]
    public Order? Order { get; set; }

    [Display(Name = nameof(PrerequisitesObtained), Prompt = nameof(PrerequisitesObtained), ResourceType = typeof(App.Resources.Domain.OperationMapping))]
    public bool PrerequisitesObtained { get; set; } = true;
    
    [Display(Name = nameof(CompletedAt), Prompt = nameof(CompletedAt), ResourceType = typeof(App.Resources.Domain.OperationMapping))]
    public DateOnly? CompletedAt { get; set; }
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Details), Prompt = nameof(Details), ResourceType = typeof(Base.Resources.Common))]
    public string? Details { get; set; }
}