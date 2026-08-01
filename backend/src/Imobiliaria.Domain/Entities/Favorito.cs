namespace Imobiliaria.Domain.Entities;

public class Favorito : BaseEntity
{
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public Guid ImovelId { get; set; }
    public Imovel Imovel { get; set; } = null!;
}
