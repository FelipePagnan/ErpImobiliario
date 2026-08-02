namespace Imobiliaria.Application.DTOs;

public class AuditoriaDto
{
    public Guid Id { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string Entidade { get; set; } = string.Empty;
    public Guid? EntidadeId { get; set; }
    public string? Detalhes { get; set; }
    public Guid? UsuarioId { get; set; }
    public string? UsuarioNome { get; set; }
    public string? UsuarioEmail { get; set; }
    public DateTime CriadoEm { get; set; }
}
