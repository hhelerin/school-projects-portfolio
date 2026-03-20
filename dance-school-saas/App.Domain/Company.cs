namespace App.Domain;

public class Company : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public bool IsActive { get; set; }
    public SubscriptionTier SubscriptionTier { get; set; }
    public string? RegistrationCode { get; set; }
    public string? VATCode { get; set; }
    public string? ContactInfo { get; set; }
    public string? Details { get; set; }
}