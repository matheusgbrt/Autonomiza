using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiraEm) GerarToken(Usuario usuario);
}
