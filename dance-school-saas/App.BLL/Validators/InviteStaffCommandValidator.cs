using App.BLL.Commands;
using FluentValidation;

namespace App.BLL.Validators;

public class InviteStaffCommandValidator : AbstractValidator<InviteStaffCommand>
{
    public InviteStaffCommandValidator()
    {
        RuleFor(x => x.Model.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Please enter a valid email address")
            .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

        RuleFor(x => x.Model.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.Model.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Model.CompanyRoleId)
            .NotEmpty().WithMessage("Please select a role");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Company ID is required");
    }
}
