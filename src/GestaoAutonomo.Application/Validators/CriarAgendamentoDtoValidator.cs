using FluentValidation;
using GestaoAutonomo.Application.DTOs.Agendamento;

namespace GestaoAutonomo.Application.Validators;

public class CriarAgendamentoDtoValidator : AbstractValidator<CriarAgendamentoDto>
{
    public CriarAgendamentoDtoValidator()
    {
        RuleFor(x => x.ClienteId).NotEmpty();
        RuleFor(x => x.ServicoId).NotEmpty();
        RuleFor(x => x.DataHoraInicio).NotEqual(default(DateTime));
        RuleFor(x => x.Observacoes).MaximumLength(2000);
    }
}
