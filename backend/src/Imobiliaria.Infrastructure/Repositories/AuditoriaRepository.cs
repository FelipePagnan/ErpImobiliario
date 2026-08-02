using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Interfaces;
using Imobiliaria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Imobiliaria.Infrastructure.Repositories;

public class AuditoriaRepository : BaseRepository<Auditoria>, IAuditoriaRepository
{
    public AuditoriaRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Auditoria>> ObterRecentesAsync(int limite = 100)
        => await _dbSet.OrderByDescending(a => a.CriadoEm).Take(limite).ToListAsync();
}