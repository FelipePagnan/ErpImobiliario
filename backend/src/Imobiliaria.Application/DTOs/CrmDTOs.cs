namespace Imobiliaria.Application.DTOs;

public class InteressadoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public Guid? ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public string? CidadeDesejada { get; set; }
    public string? BairroDesejado { get; set; }
    public int? TipoImovelDesejado { get; set; }
    public string? TipoImovelNome { get; set; }
    public int? FinalidadeDesejada { get; set; }
    public string? FinalidadeNome { get; set; }
    public decimal? OrcamentoMinimo { get; set; }
    public decimal? OrcamentoMaximo { get; set; }
    public int? DormitoriosMinimo { get; set; }
    public double? AreaMinima { get; set; }
    public string? Observacoes { get; set; }
    public bool Notificar { get; set; }
    public DateTime? UltimoContato { get; set; }
    public List<ContatoHistoricoDto>? HistoricoContatos { get; set; }
    public DateTime CriadoEm { get; set; }
    public int ImoveisCompativeis { get; set; }
}

public class InteressadoCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public Guid? ClienteId { get; set; }
    public string? CidadeDesejada { get; set; }
    public string? BairroDesejado { get; set; }
    public int? TipoImovelDesejado { get; set; }
    public int? FinalidadeDesejada { get; set; }
    public decimal? OrcamentoMinimo { get; set; }
    public decimal? OrcamentoMaximo { get; set; }
    public int? DormitoriosMinimo { get; set; }
    public double? AreaMinima { get; set; }
    public string? Observacoes { get; set; }
}

public class InteressadoUpdateDto
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? CidadeDesejada { get; set; }
    public string? BairroDesejado { get; set; }
    public int? TipoImovelDesejado { get; set; }
    public int? FinalidadeDesejada { get; set; }
    public decimal? OrcamentoMinimo { get; set; }
    public decimal? OrcamentoMaximo { get; set; }
    public int? DormitoriosMinimo { get; set; }
    public double? AreaMinima { get; set; }
    public string? Observacoes { get; set; }
    public bool? Notificar { get; set; }
}

public class ContatoHistoricoDto
{
    public DateTime Data { get; set; }
    public string Tipo { get; set; } = string.Empty; // Telefone, Email, Presencial
    public string Descricao { get; set; } = string.Empty;
}

public class ContatoRegistroDto
{
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}
