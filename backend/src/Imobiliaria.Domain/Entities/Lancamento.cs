namespace Imobiliaria.Domain.Entities;

public class Lancamento : BaseEntity
{
    public string Descricao { get; set; } = string.Empty;
    public TipoLancamento Tipo { get; set; }
    public CategoriaLancamento Categoria { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public bool Pago { get; set; } = false;

    // Vínculo opcional com contrato
    public Guid? ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    // Vínculo opcional com imóvel
    public Guid? ImovelId { get; set; }
    public Imovel? Imovel { get; set; }

    public string? Observacoes { get; set; }
}

public enum TipoLancamento
{
    Receita = 1,
    Despesa = 2
}

public enum CategoriaLancamento
{
    Aluguel = 1,
    Comissao = 2,
    Repasse = 3,
    Condominio = 4,
    IPTU = 5,
    Manutencao = 6,
    Marketing = 7,
    Administrativa = 8,
    Venda = 9,
    Outros = 10
}
