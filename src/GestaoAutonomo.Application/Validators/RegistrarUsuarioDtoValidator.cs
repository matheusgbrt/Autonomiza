using FluentValidation;
using GestaoAutonomo.Application.DTOs.Auth;

namespace GestaoAutonomo.Application.Validators;

public class RegistrarUsuarioDtoValidator : AbstractValidator<RegistrarUsuarioDto>
{
    public RegistrarUsuarioDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(8).MaximumLength(100);
    }
}
