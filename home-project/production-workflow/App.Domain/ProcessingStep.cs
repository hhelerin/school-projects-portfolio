using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class ProcessingStep : BaseEntity
{
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Name), Prompt = nameof(Name), ResourceType = typeof(App.Resources.Domain.Material))]
    public string Name { get; set; } = default!;    
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Details), Prompt = nameof(Details), ResourceType = typeof(Base.Resources.Common))]
    public string? Details { get; set; }
}