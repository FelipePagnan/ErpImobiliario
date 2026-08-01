using Imobiliaria.Domain.Enums;

namespace Imobiliaria.Application.DTOs;

public class VisitaDto
{
    public Guid Id { get; set; }
    public Guid ImovelId { get; set; }
    public string ImovelTitulo { get; set; } = string.Empty;
    public string ImovelCodigo { get; set; } = string.Empty;
    public Guid ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string? ClienteTelefone { get; set; }
    public Guid? CorretorId { get; set; }
    public string? CorretorNome { get; set; }
    public DateTime DataSolicitacao { get; set; }
    public DateTime? DataAgendada { get; set; }
    public string Status { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public string? Observacoes { get; set; }
    public string? FeedbackCliente { get; set; }
}

public class VisitaCreateDto
{
    public Guid ImovelId { get; set; }
    public Guid ClienteId { get; set; }
    public DateTime? DataAgendada { get; set; }
    public string? Observacoes { get; set; }
}

public class VisitaUpdateDto
{
    public DateTime? DataAgendada { get; set; }
    public StatusVisita? Status { get; set; }
    public Guid? CorretorId { get; set; }
    public string? Observacoes { get; set; }
    public string? FeedbackCliente { get; set; }
}
