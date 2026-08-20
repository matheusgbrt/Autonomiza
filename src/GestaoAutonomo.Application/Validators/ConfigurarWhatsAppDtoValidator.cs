using FluentValidation;
using GestaoAutonomo.Application.DTOs.Integracao;

namespace GestaoAutonomo.Application.Validators;

public class ConfigurarWhatsAppDtoValidator : AbstractValidator<ConfigurarWhatsAppDto>
{
    public ConfigurarWhatsAppDtoValidator()
    {
        RuleFor(x => x.InstanceId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Token).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ClientToken).MaximumLength(200);
    }
}
