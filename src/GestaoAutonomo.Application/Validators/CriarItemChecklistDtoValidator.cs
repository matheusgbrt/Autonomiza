using FluentValidation;
using GestaoAutonomo.Application.DTOs.Tarefa;

namespace GestaoAutonomo.Application.Validators;

public class CriarItemChecklistDtoValidator : AbstractValidator<CriarItemChecklistDto>
{
    public CriarItemChecklistDtoValidator()
    {
        RuleFor(x => x.Descricao).NotEmpty().MaximumLength(500);
    }
}
