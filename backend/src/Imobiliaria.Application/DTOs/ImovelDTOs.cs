using Imobiliaria.Domain.Enums;

namespace Imobiliaria.Application.DTOs;

// DTO de retorno
public class ImovelDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int TipoId { get; set; }
    public string Finalidade { get; set; } = string.Empty;
    public int FinalidadeId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public decimal? PrecoVenda { get; set; }
    public decimal? PrecoLocacao { get; set; }
    public decimal? ValorCondominio { get; set; }
    public decimal? ValorIPTU { get; set; }
    public double AreaTotal { get; set; }
    public double? AreaConstruida { get; set; }
    public int Dormitorios { get; set; }
    public int? Suites { get; set; }
    public int Banheiros { get; set; }
    public int VagasGaragem { get; set; }
    public int? Andares { get; set; }
    public int? Andar { get; set; }
    public bool? Mobiliado { get; set; }
    public string? FotoPrincipalUrl { get; set; }
    public List<string>? Fotos { get; set; }
    public List<string>? Caracteristicas { get; set; }
    public EnderecoDto? Endereco { get; set; }
    public string? ProprietarioNome { get; set; }
    public string? CorretorNome { get; set; }
    public string? CorretorTelefone { get; set; }
    public DateTime CriadoEm { get; set; }
}

// DTO de criação
public class ImovelCreateDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoImovel Tipo { get; set; }
    public FinalidadeImovel Finalidade { get; set; }
    public decimal? PrecoVenda { get; set; }
    public decimal? PrecoLocacao { get; set; }
    public decimal? ValorCondominio { get; set; }
    public decimal? ValorIPTU { get; set; }
    public double AreaTotal { get; set; }
    public double? AreaConstruida { get; set; }
    public int Dormitorios { get; set; }
    public int? Suites { get; set; }
    public int Banheiros { get; set; }
    public int VagasGaragem { get; set; }
    public int? Andares { get; set; }
    public int? Andar { get; set; }
    public bool? Mobiliado { get; set; }
    public string? FotoPrincipalUrl { get; set; }
    public EnderecoCreateDto Endereco { get; set; } = null!;
    public Guid ProprietarioId { get; set; }
    public Guid? CorretorId { get; set; }
    public List<string>? Caracteristicas { get; set; }
}

// DTO de atualização
public class ImovelUpdateDto
{
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public TipoImovel? Tipo { get; set; }
    public FinalidadeImovel? Finalidade { get; set; }
    public StatusImovel? Status { get; set; }
    public decimal? PrecoVenda { get; set; }
    public decimal? PrecoLocacao { get; set; }
    public decimal? ValorCondominio { get; set; }
    public decimal? ValorIPTU { get; set; }
    public double? AreaTotal { get; set; }
    public double? AreaConstruida { get; set; }
    public int? Dormitorios { get; set; }
    public int? Suites { get; set; }
    public int? Banheiros { get; set; }
    public int? VagasGaragem { get; set; }
    public int? Andares { get; set; }
    public int? Andar { get; set; }
    public bool? Mobiliado { get; set; }
    public string? FotoPrincipalUrl { get; set; }
    public Guid? CorretorId { get; set; }
    public List<string>? Caracteristicas { get; set; }
}

// DTO de filtro
public class ImovelFilterDto
{
    public string? Cidade { get; set; }
    public string? Bairro { get; set; }
    public int? Tipo { get; set; }
    public int? Finalidade { get; set; }
    public decimal? PrecoMin { get; set; }
    public decimal? PrecoMax { get; set; }
    public int? DormitoriosMin { get; set; }
    public double? AreaMin { get; set; }
    public int? VagasMin { get; set; }
}

// DTO de endereço
public class EnderecoDto
{
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
}

public class EnderecoCreateDto
{
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
}
