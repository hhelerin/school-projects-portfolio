namespace WebApp.ViewModels;

using App.DAL.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class OperationMappingCreateEditViewModel
{
    public OperationMappingDto OperationMapping { get; set; } = default!;

    [ValidateNever]
    public SelectList OrderSelectList { get; set; } = default!;

    [ValidateNever]
    public SelectList ProcessingStepSelectList { get; set; } = default!;
}
