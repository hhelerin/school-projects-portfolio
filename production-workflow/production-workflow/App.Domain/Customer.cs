using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class Customer : BaseEntity
{
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Name), Prompt = nameof(Name), ResourceType = typeof(App.Resources.Domain.Customer))]
    public string Name { get; set; } = default!;
    [MaxLength(128, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Email), Prompt = nameof(Email), ResourceType = typeof(App.Resources.Domain.Customer))]
    public string Email { get; set; } = default!;
    [MaxLength(20, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Phone), Prompt = nameof(Phone), ResourceType = typeof(App.Resources.Domain.Customer))]
    public string Phone { get; set; } = default!;
    [MaxLength(255, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(Address), Prompt = nameof(Address), ResourceType = typeof(App.Resources.Domain.Customer))]
    public string Address { get; set; } = default!;
    
    [MaxLength(255, ErrorMessageResourceType = typeof(Base.Resources.Common), ErrorMessageResourceName = "MaxLength")]
    [Display(Name = nameof(AdditionalInfo), Prompt = nameof(AdditionalInfo), ResourceType = typeof(App.Resources.Domain.Customer))]
    public string? AdditionalInfo { get; set; }
    
    [Display(Name = nameof(Orders), Prompt = nameof(Orders), ResourceType = typeof(App.Resources.Domain.Customer))]
    public ICollection<Order>? Orders { get; set; }
    
    [Display(Name = nameof(Shipments), Prompt = nameof(Shipments), ResourceType = typeof(App.Resources.Domain.Customer))]
    public ICollection<Shipment>? Shipments { get; set; }
    
    [Display(Name = nameof(AppUser), Prompt = nameof(AppUser), ResourceType = typeof(Base.Resources.Common))]
    public ICollection<CustomersUsers>? CustomersUsers { get; set; }
}