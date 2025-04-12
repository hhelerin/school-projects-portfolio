namespace WebApp.ViewModels;

using App.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class OperationMappingCreateEditViewModel
{
    public OperationMapping OperationMapping { get; set; } = default!;

    [ValidateNever]
    public SelectList OrderSelectList { get; set; } = default!;

    [ValidateNever]
    public SelectList ProcessingStepSelectList { get; set; } = default!;
}
