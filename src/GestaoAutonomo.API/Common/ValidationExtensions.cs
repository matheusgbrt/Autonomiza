using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GestaoAutonomo.API.Common;

public static class ValidationExtensions
{
    public static ModelStateDictionary ToModelState(this ValidationResult validationResult)
    {
        var modelState = new ModelStateDictionary();
        foreach (var error in validationResult.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        return modelState;
    }
}
