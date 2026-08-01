using Imobiliaria.Domain.Enums;

namespace Imobiliaria.Domain.Entities;

public class Contrato : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;

    public Guid ImovelId { get; set; }
    public Imovel Imovel { get; set; } = null!;

    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public Guid? CorretorId { get; set; }
    public Corretor? Corretor { get; set; }

    public FinalidadeImovel Tipo { get; set; } // Venda ou Locação
    public StatusContrato Status { get; set; } = StatusContrato.Ativo;

    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public DateTime? DataRescisao { get; set; }

    public decimal ValorTotal { get; set; }
    public decimal? ValorMensal { get; set; }
    public decimal? MultaRescisao { get; set; }

    public string? Observacoes { get; set; }

    // Navegação
    public ICollection<Lancamento> Lancamentos { get; set; } = new List<Lancamento>();
}
