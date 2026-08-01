using Imobiliaria.Domain.Enums;

namespace Imobiliaria.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
    public string? Telefone { get; set; }
    public DateTime? UltimoLogin { get; set; }
}
