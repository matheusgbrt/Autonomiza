using FluentValidation;
using GestaoAutonomo.Application.DTOs.Servico;

namespace GestaoAutonomo.Application.Validators;

public class CriarServicoDtoValidator : AbstractValidator<CriarServicoDto>
{
    public CriarServicoDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descricao).MaximumLength(2000);
        RuleFor(x => x.Duracao).GreaterThan(TimeSpan.Zero);
        RuleFor(x => x.ValorPadrao).GreaterThanOrEqualTo(0);
    }
}
