using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Shipment : BaseEntity
{
    [Display(Name = nameof(ShippedOn), Prompt = nameof(ShippedOn), ResourceType = typeof(App.Resources.Domain.Shipment))]
    public DateOnly ShippedOn { get; set; }
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Method), Prompt = nameof(Method), ResourceType = typeof(App.Resources.Domain.Shipment))]
    public string Method { get; set; } = default!;    
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Details), Prompt = nameof(Details), ResourceType = typeof(Base.Resources.Common))]
    public string? Details { get; set; }
    [Display(Name = nameof(Customer), Prompt = nameof(Customer), ResourceType = typeof(App.Resources.Domain.Order))]
    public Guid CustomerId { get; set; }
    [Display(Name = nameof(Customer), Prompt = nameof(Customer), ResourceType = typeof(App.Resources.Domain.Order))]
    public Customer? Customer { get; set; }
}