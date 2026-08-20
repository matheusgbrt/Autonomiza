using FluentValidation;
using GestaoAutonomo.Application.DTOs.Integracao;

namespace GestaoAutonomo.Application.Validators;

public class AtualizarConfiguracaoWhatsAppDtoValidator : AbstractValidator<AtualizarConfiguracaoWhatsAppDto>
{
    public AtualizarConfiguracaoWhatsAppDtoValidator()
    {
        RuleFor(x => x.MensagemBoasVindas).MaximumLength(1000);
    }
}
