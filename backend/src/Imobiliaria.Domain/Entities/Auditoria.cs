namespace Imobiliaria.Domain.Entities;

public class Auditoria : BaseEntity
{
    public string Acao { get; set; } = string.Empty;
    public string Entidade { get; set; } = string.Empty;
    public Guid? EntidadeId { get; set; }
    public string? Detalhes { get; set; }
    public Guid? UsuarioId { get; set; }
    public string? UsuarioNome { get; set; }
    public string? UsuarioEmail { get; set; }
    public string? IP { get; set; }
}
