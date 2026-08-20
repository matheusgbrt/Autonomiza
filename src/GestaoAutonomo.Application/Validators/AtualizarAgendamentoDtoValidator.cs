using FluentValidation;
using GestaoAutonomo.Application.DTOs.Agendamento;

namespace GestaoAutonomo.Application.Validators;

public class AtualizarAgendamentoDtoValidator : AbstractValidator<AtualizarAgendamentoDto>
{
    public AtualizarAgendamentoDtoValidator()
    {
        RuleFor(x => x.ClienteId).NotEmpty();
        RuleFor(x => x.ServicoId).NotEmpty();
        RuleFor(x => x.DataHoraInicio).NotEqual(default(DateTime));
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Observacoes).MaximumLength(2000);
    }
}
