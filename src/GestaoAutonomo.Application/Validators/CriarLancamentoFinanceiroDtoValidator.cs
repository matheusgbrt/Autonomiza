using FluentValidation;
using GestaoAutonomo.Application.DTOs.LancamentoFinanceiro;

namespace GestaoAutonomo.Application.Validators;

public class CriarLancamentoFinanceiroDtoValidator : AbstractValidator<CriarLancamentoFinanceiroDto>
{
    public CriarLancamentoFinanceiroDtoValidator()
    {
        RuleFor(x => x.Tipo).IsInEnum();
        RuleFor(x => x.Categoria).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Valor).GreaterThan(0);
        RuleFor(x => x.Data).NotEqual(default(DateTime));
        RuleFor(x => x.Descricao).MaximumLength(2000);
    }
}
