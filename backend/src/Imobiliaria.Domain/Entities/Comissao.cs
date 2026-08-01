using Imobiliaria.Domain.Enums;

namespace Imobiliaria.Domain.Entities;

public class Comissao : BaseEntity
{
    public Guid CorretorId { get; set; }
    public Corretor Corretor { get; set; } = null!;

    public Guid? ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public Guid? ImovelId { get; set; }
    public Imovel? Imovel { get; set; }

    public decimal ValorBase { get; set; }
    public decimal Percentual { get; set; }
    public decimal ValorComissao { get; set; }

    public DateTime DataCalculo { get; set; } = DateTime.UtcNow;
    public bool Pago { get; set; } = false;
    public DateTime? DataPagamento { get; set; }

    public string? Observacoes { get; set; }
}
