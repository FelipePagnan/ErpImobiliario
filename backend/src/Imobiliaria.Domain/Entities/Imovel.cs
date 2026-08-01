using Imobiliaria.Domain.Enums;

namespace Imobiliaria.Domain.Entities;

public class Imovel : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Codigo { get; set; } = string.Empty;

    // Classificação
    public TipoImovel Tipo { get; set; }
    public FinalidadeImovel Finalidade { get; set; }
    public StatusImovel Status { get; set; } = StatusImovel.Disponivel;

    // Valores
    public decimal? PrecoVenda { get; set; }
    public decimal? PrecoLocacao { get; set; }
    public decimal? ValorCondominio { get; set; }
    public decimal? ValorIPTU { get; set; }

    // Características
    public double AreaTotal { get; set; }
    public double? AreaConstruida { get; set; }
    public int Dormitorios { get; set; }
    public int? Suites { get; set; }
    public int Banheiros { get; set; }
    public int VagasGaragem { get; set; }
    public int? Andares { get; set; }
    public int? Andar { get; set; }
    public bool? Mobiliado { get; set; }

    // Imagens
    public string? FotoPrincipalUrl { get; set; }
    public string? FotosJson { get; set; } // JSON com array de URLs

    // Endereço
    public Guid EnderecoId { get; set; }
    public Endereco Endereco { get; set; } = null!;

    // Proprietário
    public Guid ProprietarioId { get; set; }
    public Proprietario Proprietario { get; set; } = null!;

    // Corretor responsável
    public Guid? CorretorId { get; set; }
    public Corretor? Corretor { get; set; }

    // Características extras (JSON flexível)
    public string? CaracteristicasJson { get; set; }
}
