namespace Imobiliaria.Domain.Entities;

public class Corretor : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string CRECI { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? FotoUrl { get; set; }

    // Vínculo com usuário do sistema
    public Guid? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    // Navegação
    public ICollection<Imovel> Imoveis { get; set; } = new List<Imovel>();
}
