using System.Linq.Expressions;
using Imobiliaria.Domain.Entities;

namespace Imobiliaria.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<T>> ObterTodosAsync();
    Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicate);
    Task<T> AdicionarAsync(T entity);
    Task AtualizarAsync(T entity);
    Task RemoverAsync(Guid id);
    Task<int> ContarAsync(Expression<Func<T, bool>>? predicate = null);
}

public interface IImovelRepository : IRepository<Imovel>
{
    Task<IEnumerable<Imovel>> ObterComDetalhesAsync();
    Task<Imovel?> ObterComDetalhesPorIdAsync(Guid id);
    Task<IEnumerable<Imovel>> FiltrarAsync(
        string? cidade = null,
        string? bairro = null,
        int? tipo = null,
        int? finalidade = null,
        decimal? precoMin = null,
        decimal? precoMax = null,
        int? dormitoriosMin = null,
        double? areaMin = null,
        int? vagasMin = null);
}

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> ObterPorEmailAsync(string email);
}

public interface IProprietarioRepository : IRepository<Proprietario>
{
    Task<Proprietario?> ObterComImoveisAsync(Guid id);
}

public interface ICorretorRepository : IRepository<Corretor>
{
    Task<Corretor?> ObterComImoveisAsync(Guid id);
}

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> ObterComFavoritosAsync(Guid id);
}

public interface IFavoritoRepository : IRepository<Favorito>
{
    Task<IEnumerable<Favorito>> ObterPorClienteAsync(Guid clienteId);
    Task<Favorito?> ObterPorClienteEImovelAsync(Guid clienteId, Guid imovelId);
}

// --- V2 ---

public interface IVisitaRepository : IRepository<Visita>
{
    Task<IEnumerable<Visita>> ObterComDetalhesAsync();
    Task<Visita?> ObterComDetalhesAsync(Guid id);
    Task<IEnumerable<Visita>> ObterPorClienteAsync(Guid clienteId);
    Task<IEnumerable<Visita>> ObterPorCorretorAsync(Guid corretorId);
    Task<IEnumerable<Visita>> ObterPorImovelAsync(Guid imovelId);
}

public interface IContratoRepository : IRepository<Contrato>
{
    Task<IEnumerable<Contrato>> ObterComDetalhesAsync();
    Task<Contrato?> ObterComDetalhesAsync(Guid id);
    Task<IEnumerable<Contrato>> ObterPorClienteAsync(Guid clienteId);
    Task<IEnumerable<Contrato>> ObterPorImovelAsync(Guid imovelId);
    Task<IEnumerable<Contrato>> ObterVencendoAsync(int dias = 30);
}

public interface ILancamentoRepository : IRepository<Lancamento>
{
    Task<IEnumerable<Lancamento>> ObterComDetalhesAsync();
    Task<IEnumerable<Lancamento>> ObterPorContratoAsync(Guid contratoId);
    Task<IEnumerable<Lancamento>> ObterPorPeriodoAsync(DateTime inicio, DateTime fim);
    Task<IEnumerable<Lancamento>> ObterPendentesAsync();
}

public interface IComissaoRepository : IRepository<Comissao>
{
    Task<IEnumerable<Comissao>> ObterComDetalhesAsync();
    Task<IEnumerable<Comissao>> ObterPorCorretorAsync(Guid corretorId);
    Task<IEnumerable<Comissao>> ObterPendentesAsync();
}

public interface IInteressadoRepository : IRepository<Interessado>
{
    Task<IEnumerable<Interessado>> ObterComDetalhesAsync();
    Task<Interessado?> ObterComDetalhesAsync(Guid id);
    Task<IEnumerable<Interessado>> BuscarPorPreferenciaAsync(
        string? cidade, string? bairro, int? tipo, int? finalidade,
        decimal? precoMin, decimal? precoMax);
}
