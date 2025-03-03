using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Order : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; } = default!;
    
    public Guid MaterialId { get; set; }
    public Material? Material { get; set; } = default!;
    
    [MaxLength(30)]
    public string OrderNumber { get; set; } = default!;
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    public DateOnly OrderDate { get; set; }
    public DateOnly Deadline { get; set; }
    public int TotalAmount { get; set; }
    public double? TotalArea { get; set; }
    public double? LinearMeter { get; set; }
    [MaxLength(128)]
    public string? Details { get; set; }

    public EStatus Status { get; set; } = EStatus.Pending;
    
    public Guid? ShipmentID { get; set; }
    public Shipment? Shipment { get; set; } = default!;
    
    public int? PalletNumber { get; set; }
    public DateOnly? BillingDate { get; set; }

}