using FluentValidation;
using GestaoAutonomo.Application.DTOs.Cliente;

namespace GestaoAutonomo.Application.Validators;

public class AtualizarClienteDtoValidator : AbstractValidator<AtualizarClienteDto>
{
    public AtualizarClienteDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Telefone).MaximumLength(20);
        RuleFor(x => x.Observacoes).MaximumLength(2000);
    }
}
