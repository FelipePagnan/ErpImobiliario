namespace Imobiliaria.Domain.Entities;

public class Interessado : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }

    // Vínculo opcional com cliente já cadastrado
    public Guid? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    // Preferências de busca
    public string? CidadeDesejada { get; set; }
    public string? BairroDesejado { get; set; }
    public int? TipoImovelDesejado { get; set; }
    public int? FinalidadeDesejada { get; set; }
    public decimal? OrcamentoMinimo { get; set; }
    public decimal? OrcamentoMaximo { get; set; }
    public int? DormitoriosMinimo { get; set; }
    public double? AreaMinima { get; set; }

    public string? Observacoes { get; set; }
    public bool Notificar { get; set; } = true;
    public DateTime? UltimoContato { get; set; }

    // Histórico de contatos (JSON)
    public string? HistoricoContatosJson { get; set; }
}
