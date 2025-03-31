using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class CustomersUsers : BaseEntity
{
    [Display(Name = nameof(Customer), Prompt = nameof(Customer), ResourceType = typeof(App.Resources.Domain.Order))]
    public Guid CustomerId { get; set; }
    [Display(Name = nameof(Customer), Prompt = nameof(Customer), ResourceType = typeof(App.Resources.Domain.Order))]
    public Customer? Customer { get; set; }
    
    [Display(Name = nameof(AppUser), Prompt = nameof(AppUser), ResourceType = typeof(Base.Resources.Common))]
    public Guid AppUserId { get; set; }
    [Display(Name = nameof(AppUser), Prompt = nameof(AppUser), ResourceType = typeof(Base.Resources.Common))]
    public AppUser? AppUser { get; set; }
}