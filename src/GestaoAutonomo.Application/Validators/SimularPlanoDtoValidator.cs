using FluentValidation;
using GestaoAutonomo.Application.DTOs.Auth;

namespace GestaoAutonomo.Application.Validators;

public class SimularPlanoDtoValidator : AbstractValidator<SimularPlanoDto>
{
    public SimularPlanoDtoValidator()
    {
        RuleFor(x => x.Plano).IsInEnum();
    }
}
