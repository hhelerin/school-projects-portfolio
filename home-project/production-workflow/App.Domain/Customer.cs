using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Customer : BaseEntity
{
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    [MaxLength(128)]
    public string Email { get; set; } = default!;
    [MaxLength(20)]
    public string Phone { get; set; } = default!;
    [MaxLength(255)]
    public string Address { get; set; } = default!;
    [MaxLength(255)]
    public string? AdditionalInfo { get; set; } = default!;
    
    public ICollection<Order>? Orders { get; set; }
    
    public ICollection<Shipment>? Shipments { get; set; }
}