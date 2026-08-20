using GestaoAutonomo.Application.DTOs.Cliente;
using GestaoAutonomo.Application.Exceptions;
using GestaoAutonomo.Application.Interfaces;
using GestaoAutonomo.Domain.Entities;

namespace GestaoAutonomo.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteDto> CriarAsync(Guid usuarioId, CriarClienteDto dto, CancellationToken ct)
    {
        var cliente = new Cliente
        {
            UsuarioId = usuarioId,
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            Observacoes = dto.Observacoes
        };

        await _clienteRepository.AdicionarAsync(cliente, ct);
        await _clienteRepository.SalvarAlteracoesAsync(ct);

        return ParaDto(cliente);
    }

    public async Task<ClienteDto?> ObterPorIdAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(usuarioId, id, ct);
        return cliente is null ? null : ParaDto(cliente);
    }

    public async Task<IReadOnlyList<ClienteDto>> ListarAsync(Guid usuarioId, CancellationToken ct)
    {
        var clientes = await _clienteRepository.ListarAsync(usuarioId, ct);
        return clientes.Select(ParaDto).ToList();
    }

    public async Task<ClienteDto> AtualizarAsync(Guid usuarioId, Guid id, AtualizarClienteDto dto, CancellationToken ct)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Cliente não encontrado.");

        cliente.Nome = dto.Nome;
        cliente.Email = dto.Email;
        cliente.Telefone = dto.Telefone;
        cliente.Observacoes = dto.Observacoes;

        await _clienteRepository.SalvarAlteracoesAsync(ct);

        return ParaDto(cliente);
    }

    public async Task RemoverAsync(Guid usuarioId, Guid id, CancellationToken ct)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(usuarioId, id, ct)
            ?? throw new RecursoNaoEncontradoException("Cliente não encontrado.");

        _clienteRepository.Remover(cliente);
        await _clienteRepository.SalvarAlteracoesAsync(ct);
    }

    private static ClienteDto ParaDto(Cliente cliente) => new(
        cliente.Id,
        cliente.Nome,
        cliente.Email,
        cliente.Telefone,
        cliente.Observacoes,
        cliente.CreatedAt);
}
