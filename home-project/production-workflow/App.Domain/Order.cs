using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Order : BaseEntity
{    
    [Display(Name = nameof(Customer), Prompt = nameof(Customer), ResourceType = typeof(App.Resources.Domain.Order))]
    public Guid CustomerId { get; set; }
    [Display(Name = nameof(Customer), Prompt = nameof(Customer), ResourceType = typeof(App.Resources.Domain.Order))]
    public Customer? Customer { get; set; }
    
    [Display(Name = nameof(Material), Prompt = nameof(Material), ResourceType = typeof(App.Resources.Domain.Order))]
    public Guid MaterialId { get; set; }
    [Display(Name = nameof(Material), Prompt = nameof(Material), ResourceType = typeof(App.Resources.Domain.Order))]
    public Material? Material { get; set; }
    
    [MaxLength(30, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(OrderNumber), Prompt = nameof(OrderNumber), ResourceType = typeof(App.Resources.Domain.Order))]
    public string OrderNumber { get; set; } = default!;
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Name), Prompt = nameof(Name), ResourceType = typeof(App.Resources.Domain.Order))]
    public string Name { get; set; } = default!;
    [Display(Name = nameof(OrderDate), Prompt = nameof(OrderDate), ResourceType = typeof(App.Resources.Domain.Order))]
    public DateOnly OrderDate { get; set; }
    [Display(Name = nameof(Deadline), Prompt = nameof(Deadline), ResourceType = typeof(App.Resources.Domain.Order))]
    public DateOnly Deadline { get; set; }
    [Display(Name = nameof(TotalAmount), Prompt = nameof(TotalAmount), ResourceType = typeof(App.Resources.Domain.Order))]
    public int TotalAmount { get; set; }
    [Display(Name = nameof(TotalArea), Prompt = nameof(TotalArea), ResourceType = typeof(App.Resources.Domain.Order))]
    public double? TotalArea { get; set; }
    [Display(Name = nameof(LinearMeter), Prompt = nameof(LinearMeter), ResourceType = typeof(App.Resources.Domain.Order))]
    public double? LinearMeter { get; set; }
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Details), Prompt = nameof(Details), ResourceType = typeof(Base.Resources.Common))]
    public string? Details { get; set; }
    [Display(Name = nameof(Status), Prompt = nameof(Status), ResourceType = typeof(App.Resources.Domain.Order))]
    public EStatus Status { get; set; } = EStatus.Pending;
    [Display(Name = nameof(Shipment), Prompt = nameof(Shipment), ResourceType = typeof(App.Resources.Domain.Order))]
    public Guid? ShipmentID { get; set; }
    
    [Display(Name = nameof(Shipment), Prompt = nameof(Shipment), ResourceType = typeof(App.Resources.Domain.Order))]
    public Shipment? Shipment { get; set; }
    
    [Display(Name = nameof(PalletNumber), Prompt = nameof(PalletNumber), ResourceType = typeof(App.Resources.Domain.Order))]
    public int? PalletNumber { get; set; }
    
    [Display(Name = nameof(BillingDate), Prompt = nameof(BillingDate), ResourceType = typeof(App.Resources.Domain.Order))]
    public DateOnly? BillingDate { get; set; }

}