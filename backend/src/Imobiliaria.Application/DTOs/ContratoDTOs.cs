using Imobiliaria.Domain.Enums;

namespace Imobiliaria.Application.DTOs;

public class ContratoDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public Guid ImovelId { get; set; }
    public string ImovelTitulo { get; set; } = string.Empty;
    public string ImovelCodigo { get; set; } = string.Empty;
    public Guid ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public Guid? CorretorId { get; set; }
    public string? CorretorNome { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int TipoId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public DateTime? DataRescisao { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal? ValorMensal { get; set; }
    public decimal? MultaRescisao { get; set; }
    public string? Observacoes { get; set; }
    public DateTime CriadoEm { get; set; }
}

public class ContratoCreateDto
{
    public Guid ImovelId { get; set; }
    public Guid ClienteId { get; set; }
    public Guid? CorretorId { get; set; }
    public FinalidadeImovel Tipo { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal? ValorMensal { get; set; }
    public decimal? MultaRescisao { get; set; }
    public string? Observacoes { get; set; }
}

public class ContratoUpdateDto
{
    public StatusContrato? Status { get; set; }
    public DateTime? DataFim { get; set; }
    public DateTime? DataRescisao { get; set; }
    public decimal? ValorMensal { get; set; }
    public decimal? MultaRescisao { get; set; }
    public string? Observacoes { get; set; }
}
