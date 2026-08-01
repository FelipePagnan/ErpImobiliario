using Imobiliaria.Domain.Entities;

namespace Imobiliaria.Application.DTOs;

public class LancamentoDto
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int TipoId { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public bool Pago { get; set; }
    public Guid? ContratoId { get; set; }
    public string? ContratoCodigo { get; set; }
    public Guid? ImovelId { get; set; }
    public string? ImovelTitulo { get; set; }
    public string? Observacoes { get; set; }
    public DateTime CriadoEm { get; set; }
}

public class LancamentoCreateDto
{
    public string Descricao { get; set; } = string.Empty;
    public TipoLancamento Tipo { get; set; }
    public CategoriaLancamento Categoria { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public Guid? ContratoId { get; set; }
    public Guid? ImovelId { get; set; }
    public string? Observacoes { get; set; }
}

public class LancamentoUpdateDto
{
    public string? Descricao { get; set; }
    public decimal? Valor { get; set; }
    public DateTime? DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public bool? Pago { get; set; }
    public string? Observacoes { get; set; }
}

public class ComissaoDto
{
    public Guid Id { get; set; }
    public Guid CorretorId { get; set; }
    public string CorretorNome { get; set; } = string.Empty;
    public Guid? ContratoId { get; set; }
    public string? ContratoCodigo { get; set; }
    public Guid? ImovelId { get; set; }
    public string? ImovelTitulo { get; set; }
    public decimal ValorBase { get; set; }
    public decimal Percentual { get; set; }
    public decimal ValorComissao { get; set; }
    public DateTime DataCalculo { get; set; }
    public bool Pago { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string? Observacoes { get; set; }
}

public class ComissaoCreateDto
{
    public Guid CorretorId { get; set; }
    public Guid? ContratoId { get; set; }
    public Guid? ImovelId { get; set; }
    public decimal ValorBase { get; set; }
    public decimal Percentual { get; set; }
    public string? Observacoes { get; set; }
}

public class ResumoFinanceiroDto
{
    public decimal ReceitaTotal { get; set; }
    public decimal DespesaTotal { get; set; }
    public decimal Saldo { get; set; }
    public decimal ReceitaPendente { get; set; }
    public decimal DespesaPendente { get; set; }
    public decimal ComissoesPendentes { get; set; }
    public int LancamentosVencidos { get; set; }
    public List<LancamentoDto> UltimosLancamentos { get; set; } = new();
}
