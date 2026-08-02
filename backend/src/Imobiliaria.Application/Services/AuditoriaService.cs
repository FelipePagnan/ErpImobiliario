using Imobiliaria.Application.DTOs;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Interfaces;

namespace Imobiliaria.Application.Services;

public interface IAuditoriaService
{
    Task RegistrarAsync(string acao, string entidade, Guid? entidadeId, string? detalhes,
        Guid? usuarioId, string? usuarioNome, string? usuarioEmail);
    Task<IEnumerable<AuditoriaDto>> ObterTodosAsync(int limite = 100);
    Task<IEnumerable<AuditoriaDto>> ObterPorEntidadeAsync(string entidade, Guid entidadeId);
}

public class AuditoriaService : IAuditoriaService
{
    private readonly IAuditoriaRepository _repo;
    public AuditoriaService(IAuditoriaRepository repo) => _repo = repo;

    public async Task RegistrarAsync(string acao, string entidade, Guid? entidadeId,
        string? detalhes, Guid? usuarioId, string? usuarioNome, string? usuarioEmail)
    {
        await _repo.AdicionarAsync(new Auditoria
        {
            Acao = acao, Entidade = entidade, EntidadeId = entidadeId,
            Detalhes = detalhes, UsuarioId = usuarioId,
            UsuarioNome = usuarioNome, UsuarioEmail = usuarioEmail
        });
    }

    public async Task<IEnumerable<AuditoriaDto>> ObterTodosAsync(int limite = 100)
    {
        var items = await _repo.ObterRecentesAsync(limite);
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<AuditoriaDto>> ObterPorEntidadeAsync(string entidade, Guid entidadeId)
    {
        var items = await _repo.BuscarAsync(a => a.Entidade == entidade && a.EntidadeId == entidadeId);
        return items.OrderByDescending(a => a.CriadoEm).Select(MapToDto);
    }

    private static AuditoriaDto MapToDto(Auditoria a) => new()
    {
        Id = a.Id, Acao = a.Acao, Entidade = a.Entidade, EntidadeId = a.EntidadeId,
        Detalhes = a.Detalhes, UsuarioId = a.UsuarioId,
        UsuarioNome = a.UsuarioNome, UsuarioEmail = a.UsuarioEmail, CriadoEm = a.CriadoEm
    };
}
