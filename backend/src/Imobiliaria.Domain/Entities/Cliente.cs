namespace Imobiliaria.Domain.Entities;

public class Cliente : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string? CPFouCNPJ { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Observacoes { get; set; }

    // Vínculo com usuário do sistema (login no portal)
    public Guid? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    // Preferências CRM (JSON)
    public string? PreferenciasJson { get; set; }

    // Navegação
    public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
}
