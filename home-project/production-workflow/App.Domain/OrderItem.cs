using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
    
    public int DetailNumber { get; set; }
    public int LengthMm { get; set; }
    public int WidthMm { get; set; }
    public int? HeightMm { get; set; }
    public double? Area { get; set; }
    public double? LinearMeter { get; set; }
    public int Amount { get; set; }
    [MaxLength(128)]
    public string? Details { get; set; }
}