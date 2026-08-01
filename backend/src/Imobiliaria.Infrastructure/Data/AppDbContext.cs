using Imobiliaria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Imobiliaria.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Imovel> Imoveis => Set<Imovel>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();
    public DbSet<Proprietario> Proprietarios => Set<Proprietario>();
    public DbSet<Corretor> Corretores => Set<Corretor>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Favorito> Favoritos => Set<Favorito>();

    // V2
    public DbSet<Visita> Visitas => Set<Visita>();
    public DbSet<Contrato> Contratos => Set<Contrato>();
    public DbSet<Lancamento> Lancamentos => Set<Lancamento>();
    public DbSet<Comissao> Comissoes => Set<Comissao>();
    public DbSet<Interessado> Interessados => Set<Interessado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Imovel
        modelBuilder.Entity<Imovel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Titulo).IsRequired().HasMaxLength(200);
            e.Property(x => x.Codigo).IsRequired().HasMaxLength(30);
            e.Property(x => x.PrecoVenda).HasColumnType("decimal(18,2)");
            e.Property(x => x.PrecoLocacao).HasColumnType("decimal(18,2)");
            e.Property(x => x.ValorCondominio).HasColumnType("decimal(18,2)");
            e.Property(x => x.ValorIPTU).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Endereco).WithMany().HasForeignKey(x => x.EnderecoId);
            e.HasOne(x => x.Proprietario).WithMany(p => p.Imoveis).HasForeignKey(x => x.ProprietarioId);
            e.HasOne(x => x.Corretor).WithMany(c => c.Imoveis).HasForeignKey(x => x.CorretorId);
            e.HasIndex(x => x.Codigo).IsUnique();
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.Tipo);
        });

        // Endereco
        modelBuilder.Entity<Endereco>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Logradouro).IsRequired().HasMaxLength(200);
            e.Property(x => x.Cidade).IsRequired().HasMaxLength(100);
            e.Property(x => x.Estado).IsRequired().HasMaxLength(2);
            e.Property(x => x.CEP).IsRequired().HasMaxLength(10);
        });

        // Proprietario
        modelBuilder.Entity<Proprietario>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            e.Property(x => x.CPFouCNPJ).IsRequired().HasMaxLength(20);
        });

        // Corretor
        modelBuilder.Entity<Corretor>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            e.Property(x => x.CRECI).IsRequired().HasMaxLength(20);
            e.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId);
        });

        // Cliente
        modelBuilder.Entity<Cliente>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            e.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId);
        });

        // Usuario
        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            e.Property(x => x.Email).IsRequired().HasMaxLength(200);
            e.Property(x => x.SenhaHash).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
        });

        // Favorito
        modelBuilder.Entity<Favorito>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Cliente).WithMany(c => c.Favoritos).HasForeignKey(x => x.ClienteId);
            e.HasOne(x => x.Imovel).WithMany().HasForeignKey(x => x.ImovelId);
            e.HasIndex(x => new { x.ClienteId, x.ImovelId }).IsUnique();
        });

        // === V2 ===

        // Visita
        modelBuilder.Entity<Visita>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Imovel).WithMany().HasForeignKey(x => x.ImovelId);
            e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId);
            e.HasOne(x => x.Corretor).WithMany().HasForeignKey(x => x.CorretorId);
            e.HasIndex(x => x.Status);
        });

        // Contrato
        modelBuilder.Entity<Contrato>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Codigo).IsRequired().HasMaxLength(30);
            e.Property(x => x.ValorTotal).HasColumnType("decimal(18,2)");
            e.Property(x => x.ValorMensal).HasColumnType("decimal(18,2)");
            e.Property(x => x.MultaRescisao).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Imovel).WithMany().HasForeignKey(x => x.ImovelId);
            e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId);
            e.HasOne(x => x.Corretor).WithMany().HasForeignKey(x => x.CorretorId);
            e.HasIndex(x => x.Codigo).IsUnique();
            e.HasIndex(x => x.Status);
        });

        // Lancamento
        modelBuilder.Entity<Lancamento>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Descricao).IsRequired().HasMaxLength(300);
            e.Property(x => x.Valor).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Contrato).WithMany(c => c.Lancamentos).HasForeignKey(x => x.ContratoId);
            e.HasOne(x => x.Imovel).WithMany().HasForeignKey(x => x.ImovelId);
            e.HasIndex(x => x.DataVencimento);
            e.HasIndex(x => x.Pago);
        });

        // Comissao
        modelBuilder.Entity<Comissao>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ValorBase).HasColumnType("decimal(18,2)");
            e.Property(x => x.ValorComissao).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Corretor).WithMany().HasForeignKey(x => x.CorretorId);
            e.HasOne(x => x.Contrato).WithMany().HasForeignKey(x => x.ContratoId);
            e.HasOne(x => x.Imovel).WithMany().HasForeignKey(x => x.ImovelId);
        });

        // Interessado
        modelBuilder.Entity<Interessado>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            e.Property(x => x.OrcamentoMinimo).HasColumnType("decimal(18,2)");
            e.Property(x => x.OrcamentoMaximo).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId);
        });
    }
}
