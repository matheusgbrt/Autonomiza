using FluentValidation;
using GestaoAutonomo.Application.DTOs.Tarefa;

namespace GestaoAutonomo.Application.Validators;

public class AtualizarTarefaDtoValidator : AbstractValidator<AtualizarTarefaDto>
{
    public AtualizarTarefaDtoValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descricao).MaximumLength(2000);
    }
}
