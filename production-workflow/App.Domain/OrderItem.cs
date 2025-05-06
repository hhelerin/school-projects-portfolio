using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class OrderItem : BaseEntity
{
    [Display(Name = nameof(Order), Prompt = nameof(Order), ResourceType = typeof(Base.Resources.Common))]
    public Guid OrderId { get; set; }
    
    [Display(Name = nameof(Order), Prompt = nameof(Order), ResourceType = typeof(Base.Resources.Common))]
    public Order? Order { get; set; }
    
    [Display(Name = nameof(DetailNumber), Prompt = nameof(DetailNumber), ResourceType = typeof(App.Resources.Domain.OrderItem))]
    public int DetailNumber { get; set; }
    
    [Display(Name = nameof(LengthMm), Prompt = nameof(LengthMm), ResourceType = typeof(App.Resources.Domain.OrderItem))]
    public int LengthMm { get; set; }
    
    [Display(Name = nameof(WidthMm), Prompt = nameof(WidthMm), ResourceType = typeof(App.Resources.Domain.OrderItem))]
    public int WidthMm { get; set; }
    
    [Display(Name = nameof(HeightMm), Prompt = nameof(HeightMm), ResourceType = typeof(App.Resources.Domain.OrderItem))]
    public int? HeightMm { get; set; }
    
    [Display(Name = nameof(Area), Prompt = nameof(Area), ResourceType = typeof(App.Resources.Domain.OrderItem))]
    public double? Area { get; set; }
    
    [Display(Name = nameof(LinearMeter), Prompt = nameof(LinearMeter), ResourceType = typeof(App.Resources.Domain.OrderItem))]
    public double? LinearMeter { get; set; }
    
    [Display(Name = nameof(Amount), Prompt = nameof(Amount), ResourceType = typeof(App.Resources.Domain.OrderItem))]
    public int Amount { get; set; }
    
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Details), Prompt = nameof(Details), ResourceType = typeof(Base.Resources.Common))]
    public string? Details { get; set; }
}