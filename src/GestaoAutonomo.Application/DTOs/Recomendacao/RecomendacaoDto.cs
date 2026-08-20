using GestaoAutonomo.Domain.Enums;

namespace GestaoAutonomo.Application.DTOs.Recomendacao;

public record RecomendacaoDto(CategoriaRecomendacao Categoria, string Mensagem);
