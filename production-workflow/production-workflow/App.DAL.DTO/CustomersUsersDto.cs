using System.ComponentModel.DataAnnotations;
using App.Domain;
using Contracts;

namespace App.DAL.DTO;


public class CustomersUsersDto : IDomainId
{
    public Guid Id { get; set; } 
    [Display(Name = nameof(Customer), Prompt = nameof(Customer), ResourceType = typeof(App.Resources.Domain.Order))]
    public Guid CustomerId { get; set; }
    [Display(Name = nameof(Customer), Prompt = nameof(Customer), ResourceType = typeof(App.Resources.Domain.Order))]
    public CustomerDto? Customer { get; set; }
}

