using System.Linq.Expressions;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Enums;
using Imobiliaria.Domain.Interfaces;
using Imobiliaria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Imobiliaria.Infrastructure.Repositories;

// --- Base Repository ---
public class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> ObterPorIdAsync(Guid id)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id);

    public async Task<IEnumerable<T>> ObterTodosAsync()
        => await _dbSet.Where(e => e.Ativo).ToListAsync();

    public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();

    public async Task<T> AdicionarAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task AtualizarAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Guid id)
    {
        var entity = await ObterPorIdAsync(id);
        if (entity != null)
        {
            entity.Ativo = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> ContarAsync(Expression<Func<T, bool>>? predicate = null)
        => predicate != null
            ? await _dbSet.CountAsync(predicate)
            : await _dbSet.CountAsync();
}

// --- Imovel Repository ---
public class ImovelRepository : BaseRepository<Imovel>, IImovelRepository
{
    public ImovelRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Imovel>> ObterComDetalhesAsync()
        => await _dbSet
            .Include(i => i.Endereco)
            .Include(i => i.Proprietario)
            .Include(i => i.Corretor)
            .Where(i => i.Ativo)
            .OrderByDescending(i => i.CriadoEm)
            .ToListAsync();

    public async Task<Imovel?> ObterComDetalhesPorIdAsync(Guid id)
        => await _dbSet
            .Include(i => i.Endereco)
            .Include(i => i.Proprietario)
            .Include(i => i.Corretor)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IEnumerable<Imovel>> FiltrarAsync(
        string? cidade, string? bairro, int? tipo, int? finalidade,
        decimal? precoMin, decimal? precoMax, int? dormitoriosMin,
        double? areaMin, int? vagasMin)
    {
        var query = _dbSet
            .Include(i => i.Endereco)
            .Include(i => i.Proprietario)
            .Include(i => i.Corretor)
            .Where(i => i.Ativo && i.Status == StatusImovel.Disponivel)
            .AsQueryable();

        if (!string.IsNullOrEmpty(cidade))
            query = query.Where(i => i.Endereco.Cidade.ToLower().Contains(cidade.ToLower()));
        if (!string.IsNullOrEmpty(bairro))
            query = query.Where(i => i.Endereco.Bairro.ToLower().Contains(bairro.ToLower()));
        if (tipo.HasValue)
            query = query.Where(i => (int)i.Tipo == tipo.Value);
        if (finalidade.HasValue)
            query = query.Where(i => (int)i.Finalidade == finalidade.Value);
        if (precoMin.HasValue)
            query = query.Where(i =>
                (i.PrecoVenda.HasValue && i.PrecoVenda >= precoMin) ||
                (i.PrecoLocacao.HasValue && i.PrecoLocacao >= precoMin));
        if (precoMax.HasValue)
            query = query.Where(i =>
                (i.PrecoVenda.HasValue && i.PrecoVenda <= precoMax) ||
                (i.PrecoLocacao.HasValue && i.PrecoLocacao <= precoMax));
        if (dormitoriosMin.HasValue)
            query = query.Where(i => i.Dormitorios >= dormitoriosMin.Value);
        if (areaMin.HasValue)
            query = query.Where(i => i.AreaTotal >= areaMin.Value);
        if (vagasMin.HasValue)
            query = query.Where(i => i.VagasGaragem >= vagasMin.Value);

        return await query.OrderByDescending(i => i.CriadoEm).ToListAsync();
    }
}

// --- Usuario Repository ---
public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext context) : base(context) { }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
        => await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.Ativo);
}

// --- Proprietario Repository ---
public class ProprietarioRepository : BaseRepository<Proprietario>, IProprietarioRepository
{
    public ProprietarioRepository(AppDbContext context) : base(context) { }

    public async Task<Proprietario?> ObterComImoveisAsync(Guid id)
        => await _dbSet.Include(p => p.Imoveis).FirstOrDefaultAsync(p => p.Id == id);
}

// --- Corretor Repository ---
public class CorretorRepository : BaseRepository<Corretor>, ICorretorRepository
{
    public CorretorRepository(AppDbContext context) : base(context) { }

    public async Task<Corretor?> ObterComImoveisAsync(Guid id)
        => await _dbSet.Include(c => c.Imoveis).FirstOrDefaultAsync(c => c.Id == id);
}

// --- Cliente Repository ---
public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(AppDbContext context) : base(context) { }

    public async Task<Cliente?> ObterComFavoritosAsync(Guid id)
        => await _dbSet
            .Include(c => c.Favoritos)
                .ThenInclude(f => f.Imovel)
                    .ThenInclude(i => i.Endereco)
            .FirstOrDefaultAsync(c => c.Id == id);
}

// --- Favorito Repository ---
public class FavoritoRepository : BaseRepository<Favorito>, IFavoritoRepository
{
    public FavoritoRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Favorito>> ObterPorClienteAsync(Guid clienteId)
        => await _dbSet
            .Include(f => f.Imovel)
                .ThenInclude(i => i.Endereco)
            .Where(f => f.ClienteId == clienteId && f.Ativo)
            .ToListAsync();

    public async Task<Favorito?> ObterPorClienteEImovelAsync(Guid clienteId, Guid imovelId)
        => await _dbSet.FirstOrDefaultAsync(f => f.ClienteId == clienteId && f.ImovelId == imovelId);
}
