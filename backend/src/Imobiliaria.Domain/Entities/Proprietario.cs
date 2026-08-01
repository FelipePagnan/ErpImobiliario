namespace Imobiliaria.Domain.Entities;

public class Proprietario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string CPFouCNPJ { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }

    // Navegação
    public ICollection<Imovel> Imoveis { get; set; } = new List<Imovel>();
}
