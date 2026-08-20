using FluentValidation;
using GestaoAutonomo.Application.DTOs.Tarefa;

namespace GestaoAutonomo.Application.Validators;

public class CriarTarefaDtoValidator : AbstractValidator<CriarTarefaDto>
{
    public CriarTarefaDtoValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descricao).MaximumLength(2000);
        RuleForEach(x => x.ItensIniciais).NotEmpty().MaximumLength(500);
    }
}
