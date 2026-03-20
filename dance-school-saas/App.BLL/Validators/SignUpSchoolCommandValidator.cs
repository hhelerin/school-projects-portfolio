using App.BLL.Commands;
using FluentValidation;

namespace App.BLL.Validators;

public class SignUpSchoolCommandValidator : AbstractValidator<SignUpSchoolCommand>
{
    public SignUpSchoolCommandValidator()
    {
        RuleFor(x => x.Model.OwnerFirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.Model.OwnerLastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Model.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Please enter a valid email address")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

        RuleFor(x => x.Model.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit");

        RuleFor(x => x.Model.ConfirmPassword)
            .NotEmpty().WithMessage("Please confirm your password")
            .Equal(x => x.Model.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.Model.SchoolName)
            .NotEmpty().WithMessage("School name is required")
            .MaximumLength(200).WithMessage("School name cannot exceed 200 characters");

        RuleFor(x => x.Model.Slug)
            .NotEmpty().WithMessage("School URL is required")
            .MinimumLength(3).WithMessage("URL must be at least 3 characters")
            .MaximumLength(64).WithMessage("URL cannot exceed 64 characters")
            .Matches(@"^[a-z0-9-]+$").WithMessage("URL can only contain lowercase letters, numbers, and hyphens");

        RuleFor(x => x.Model.RegistrationCode)
            .MaximumLength(50).WithMessage("Registration code cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Model.RegistrationCode));

        RuleFor(x => x.Model.VATCode)
            .MaximumLength(50).WithMessage("VAT code cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Model.VATCode));
    }
}
