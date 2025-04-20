using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;

namespace App.Domain;

public class CustomersUsers : BaseEntityUser<AppUser>
{
    [Display(Name = nameof(Customer), Prompt = nameof(Customer), ResourceType = typeof(App.Resources.Domain.Order))]
    public Guid CustomerId { get; set; }
    [Display(Name = nameof(Customer), Prompt = nameof(Customer), ResourceType = typeof(App.Resources.Domain.Order))]
    public Customer? Customer { get; set; }
    
}