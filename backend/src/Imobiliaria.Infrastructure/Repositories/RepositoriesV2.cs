using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Enums;
using Imobiliaria.Domain.Interfaces;
using Imobiliaria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Imobiliaria.Infrastructure.Repositories;

// --- Visita Repository ---
public class VisitaRepository : BaseRepository<Visita>, IVisitaRepository
{
    public VisitaRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Visita>> ObterComDetalhesAsync()
        => await _dbSet.Include(v => v.Imovel).Include(v => v.Cliente).Include(v => v.Corretor)
            .Where(v => v.Ativo).OrderByDescending(v => v.CriadoEm).ToListAsync();

    async Task<Visita?> IVisitaRepository.ObterComDetalhesAsync(Guid id)
        => await _dbSet.Include(v => v.Imovel).Include(v => v.Cliente).Include(v => v.Corretor)
            .FirstOrDefaultAsync(v => v.Id == id);

    public async Task<IEnumerable<Visita>> ObterPorClienteAsync(Guid clienteId)
        => await _dbSet.Include(v => v.Imovel).Include(v => v.Corretor)
            .Where(v => v.ClienteId == clienteId && v.Ativo).OrderByDescending(v => v.CriadoEm).ToListAsync();

    public async Task<IEnumerable<Visita>> ObterPorCorretorAsync(Guid corretorId)
        => await _dbSet.Include(v => v.Imovel).Include(v => v.Cliente)
            .Where(v => v.CorretorId == corretorId && v.Ativo).OrderByDescending(v => v.CriadoEm).ToListAsync();

    public async Task<IEnumerable<Visita>> ObterPorImovelAsync(Guid imovelId)
        => await _dbSet.Include(v => v.Cliente).Include(v => v.Corretor)
            .Where(v => v.ImovelId == imovelId && v.Ativo).ToListAsync();
}

// --- Contrato Repository ---
public class ContratoRepository : BaseRepository<Contrato>, IContratoRepository
{
    public ContratoRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Contrato>> ObterComDetalhesAsync()
        => await _dbSet.Include(c => c.Imovel).Include(c => c.Cliente).Include(c => c.Corretor)
            .Where(c => c.Ativo).OrderByDescending(c => c.CriadoEm).ToListAsync();

    async Task<Contrato?> IContratoRepository.ObterComDetalhesAsync(Guid id)
        => await _dbSet.Include(c => c.Imovel).Include(c => c.Cliente).Include(c => c.Corretor)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Contrato>> ObterPorClienteAsync(Guid clienteId)
        => await _dbSet.Include(c => c.Imovel).Where(c => c.ClienteId == clienteId && c.Ativo).ToListAsync();

    public async Task<IEnumerable<Contrato>> ObterPorImovelAsync(Guid imovelId)
        => await _dbSet.Include(c => c.Cliente).Where(c => c.ImovelId == imovelId && c.Ativo).ToListAsync();

    public async Task<IEnumerable<Contrato>> ObterVencendoAsync(int dias = 30)
    {
        var limite = DateTime.UtcNow.AddDays(dias);
        return await _dbSet.Include(c => c.Imovel).Include(c => c.Cliente).Include(c => c.Corretor)
            .Where(c => c.Ativo && c.Status == StatusContrato.Ativo && c.DataFim.HasValue && c.DataFim <= limite)
            .OrderBy(c => c.DataFim).ToListAsync();
    }
}

// --- Lancamento Repository ---
public class LancamentoRepository : BaseRepository<Lancamento>, ILancamentoRepository
{
    public LancamentoRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Lancamento>> ObterComDetalhesAsync()
        => await _dbSet.Include(l => l.Contrato).Include(l => l.Imovel)
            .Where(l => l.Ativo).OrderByDescending(l => l.DataVencimento).ToListAsync();

    public async Task<IEnumerable<Lancamento>> ObterPorContratoAsync(Guid contratoId)
        => await _dbSet.Where(l => l.ContratoId == contratoId && l.Ativo).OrderBy(l => l.DataVencimento).ToListAsync();

    public async Task<IEnumerable<Lancamento>> ObterPorPeriodoAsync(DateTime inicio, DateTime fim)
        => await _dbSet.Include(l => l.Contrato).Include(l => l.Imovel)
            .Where(l => l.Ativo && l.DataVencimento >= inicio && l.DataVencimento <= fim)
            .OrderBy(l => l.DataVencimento).ToListAsync();

    public async Task<IEnumerable<Lancamento>> ObterPendentesAsync()
        => await _dbSet.Include(l => l.Contrato).Include(l => l.Imovel)
            .Where(l => l.Ativo && !l.Pago).OrderBy(l => l.DataVencimento).ToListAsync();
}

// --- Comissao Repository ---
public class ComissaoRepository : BaseRepository<Comissao>, IComissaoRepository
{
    public ComissaoRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Comissao>> ObterComDetalhesAsync()
        => await _dbSet.Include(c => c.Corretor).Include(c => c.Contrato).Include(c => c.Imovel)
            .Where(c => c.Ativo).OrderByDescending(c => c.DataCalculo).ToListAsync();

    public async Task<IEnumerable<Comissao>> ObterPorCorretorAsync(Guid corretorId)
        => await _dbSet.Include(c => c.Contrato).Include(c => c.Imovel)
            .Where(c => c.CorretorId == corretorId && c.Ativo).ToListAsync();

    public async Task<IEnumerable<Comissao>> ObterPendentesAsync()
        => await _dbSet.Include(c => c.Corretor)
            .Where(c => c.Ativo && !c.Pago).ToListAsync();
}

// --- Interessado Repository ---
public class InteressadoRepository : BaseRepository<Interessado>, IInteressadoRepository
{
    public InteressadoRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Interessado>> ObterComDetalhesAsync()
        => await _dbSet.Include(i => i.Cliente).Where(i => i.Ativo).OrderByDescending(i => i.CriadoEm).ToListAsync();

    async Task<Interessado?> IInteressadoRepository.ObterComDetalhesAsync(Guid id)
        => await _dbSet.Include(i => i.Cliente).FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IEnumerable<Interessado>> BuscarPorPreferenciaAsync(
        string? cidade, string? bairro, int? tipo, int? finalidade, decimal? precoMin, decimal? precoMax)
    {
        var query = _dbSet.Where(i => i.Ativo && i.Notificar).AsQueryable();
        if (!string.IsNullOrEmpty(cidade))
            query = query.Where(i => i.CidadeDesejada != null && i.CidadeDesejada.ToLower().Contains(cidade.ToLower()));
        if (tipo.HasValue)
            query = query.Where(i => i.TipoImovelDesejado == tipo);
        if (finalidade.HasValue)
            query = query.Where(i => i.FinalidadeDesejada == finalidade);
        return await query.ToListAsync();
    }
}
