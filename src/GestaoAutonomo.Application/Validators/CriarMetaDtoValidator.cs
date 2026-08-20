using FluentValidation;
using GestaoAutonomo.Application.DTOs.Meta;

namespace GestaoAutonomo.Application.Validators;

public class CriarMetaDtoValidator : AbstractValidator<CriarMetaDto>
{
    public CriarMetaDtoValidator()
    {
        RuleFor(x => x.Tipo).IsInEnum();
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ValorAlvo).GreaterThan(0);
        RuleFor(x => x.PeriodoFim).GreaterThan(x => x.PeriodoInicio);
    }
}
