using FluentValidation;
using GestaoAutonomo.Application.DTOs.Tarefa;

namespace GestaoAutonomo.Application.Validators;

public class AtualizarItemChecklistDtoValidator : AbstractValidator<AtualizarItemChecklistDto>
{
    public AtualizarItemChecklistDtoValidator()
    {
        RuleFor(x => x.Descricao).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Ordem).GreaterThanOrEqualTo(0);
    }
}
