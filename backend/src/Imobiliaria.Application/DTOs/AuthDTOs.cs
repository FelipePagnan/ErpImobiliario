using Imobiliaria.Domain.Enums;

namespace Imobiliaria.Application.DTOs;

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class RegisterDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Cliente;
}

public class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiracao { get; set; }
    public UsuarioDto Usuario { get; set; } = null!;
}

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public int PerfilId { get; set; }
}

public class DashboardDto
{
    public int ImoveisDisponiveis { get; set; }
    public int ImoveisAlugados { get; set; }
    public int ImoveisVendidos { get; set; }
    public int TotalClientes { get; set; }
    public int TotalCorretores { get; set; }
    public int TotalProprietarios { get; set; }
    public List<ImovelDto> ImoveisRecentes { get; set; } = new();
}
