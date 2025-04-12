using App.Domain;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class CustomersUsersCreateEditViewModel
{
    public CustomersUsers CustomersUsers { get; set; } = default!;

    [ValidateNever]
    public SelectList CustomerSelectList { get; set; } = default!;

    [ValidateNever]
    public SelectList UserSelectList { get; set; } = default!;
}