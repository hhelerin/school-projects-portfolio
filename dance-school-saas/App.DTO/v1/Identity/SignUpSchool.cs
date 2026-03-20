namespace App.DTO.v1.Identity;

public class SignUpSchoolInputModel
{
    public string OwnerFirstName { get; set; } = default!;
    public string OwnerLastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
    public string SchoolName { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? RegistrationCode { get; set; }
    public string? VATCode { get; set; }
}

public class SignUpSchoolResult
{
    public Guid CompanyId { get; set; }
    public string Slug { get; set; } = default!;
}
