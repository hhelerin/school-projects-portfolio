using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Shipment : BaseEntity
{
    public DateOnly ShippedOn { get; set; }
    [MaxLength(128)]
    public string Method { get; set; } = default!;    
    [MaxLength(128)]
    public string? Details { get; set; } = default!;
    
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; } = default!;
}