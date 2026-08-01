using Imobiliaria.Domain.Enums;

namespace Imobiliaria.Domain.Entities;

public class Visita : BaseEntity
{
    public Guid ImovelId { get; set; }
    public Imovel Imovel { get; set; } = null!;

    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public Guid? CorretorId { get; set; }
    public Corretor? Corretor { get; set; }

    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAgendada { get; set; }
    public StatusVisita Status { get; set; } = StatusVisita.Solicitada;
    public string? Observacoes { get; set; }
    public string? FeedbackCliente { get; set; }
}
