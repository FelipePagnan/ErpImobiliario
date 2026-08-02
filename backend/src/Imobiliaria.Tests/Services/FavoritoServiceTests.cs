using FluentAssertions;
using Imobiliaria.Application.Services;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Interfaces;
using Moq;
using Xunit;

namespace Imobiliaria.Tests.Services;

public class FavoritoServiceTests
{
    private readonly Mock<IFavoritoRepository> _favRepoMock;
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly FavoritoService _service;

    public FavoritoServiceTests()
    {
        _favRepoMock = new Mock<IFavoritoRepository>();
        _imovelRepoMock = new Mock<IImovelRepository>();
        _service = new FavoritoService(_favRepoMock.Object, _imovelRepoMock.Object);
    }

    [Fact]
    public async Task AdicionarFavoritoAsync_Novo_DeveCriarFavorito()
    {
        var clienteId = Guid.NewGuid();
        var imovelId = Guid.NewGuid();
        _favRepoMock.Setup(r => r.ObterPorClienteEImovelAsync(clienteId, imovelId)).ReturnsAsync((Favorito?)null);
        _favRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Favorito>())).ReturnsAsync((Favorito f) => f);

        var resultado = await _service.AdicionarFavoritoAsync(clienteId, imovelId);

        resultado.Should().BeTrue();
        _favRepoMock.Verify(r => r.AdicionarAsync(It.Is<Favorito>(f => f.ClienteId == clienteId && f.ImovelId == imovelId)), Times.Once);
    }

    [Fact]
    public async Task AdicionarFavoritoAsync_JaExiste_NaoDeveDuplicar()
    {
        var clienteId = Guid.NewGuid();
        var imovelId = Guid.NewGuid();
        var existente = new Favorito { ClienteId = clienteId, ImovelId = imovelId, Ativo = true };
        _favRepoMock.Setup(r => r.ObterPorClienteEImovelAsync(clienteId, imovelId)).ReturnsAsync(existente);

        var resultado = await _service.AdicionarFavoritoAsync(clienteId, imovelId);

        resultado.Should().BeTrue();
        _favRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Favorito>()), Times.Never);
    }

    [Fact]
    public async Task AdicionarFavoritoAsync_InativoExistente_DeveReativar()
    {
        var clienteId = Guid.NewGuid();
        var imovelId = Guid.NewGuid();
        var inativo = new Favorito { ClienteId = clienteId, ImovelId = imovelId, Ativo = false };
        _favRepoMock.Setup(r => r.ObterPorClienteEImovelAsync(clienteId, imovelId)).ReturnsAsync(inativo);

        await _service.AdicionarFavoritoAsync(clienteId, imovelId);

        inativo.Ativo.Should().BeTrue();
        _favRepoMock.Verify(r => r.AtualizarAsync(inativo), Times.Once);
    }

    [Fact]
    public async Task RemoverFavoritoAsync_Existente_DeveDesativar()
    {
        var clienteId = Guid.NewGuid();
        var imovelId = Guid.NewGuid();
        var fav = new Favorito { ClienteId = clienteId, ImovelId = imovelId, Ativo = true };
        _favRepoMock.Setup(r => r.ObterPorClienteEImovelAsync(clienteId, imovelId)).ReturnsAsync(fav);

        var resultado = await _service.RemoverFavoritoAsync(clienteId, imovelId);

        resultado.Should().BeTrue();
        fav.Ativo.Should().BeFalse();
    }

    [Fact]
    public async Task RemoverFavoritoAsync_Inexistente_DeveRetornarFalse()
    {
        _favRepoMock.Setup(r => r.ObterPorClienteEImovelAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Favorito?)null);

        var resultado = await _service.RemoverFavoritoAsync(Guid.NewGuid(), Guid.NewGuid());

        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task EhFavoritoAsync_Ativo_DeveRetornarTrue()
    {
        var fav = new Favorito { Ativo = true };
        _favRepoMock.Setup(r => r.ObterPorClienteEImovelAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(fav);

        var resultado = await _service.EhFavoritoAsync(Guid.NewGuid(), Guid.NewGuid());

        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task EhFavoritoAsync_Inativo_DeveRetornarFalse()
    {
        var fav = new Favorito { Ativo = false };
        _favRepoMock.Setup(r => r.ObterPorClienteEImovelAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(fav);

        var resultado = await _service.EhFavoritoAsync(Guid.NewGuid(), Guid.NewGuid());

        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ObterFavoritosAsync_DeveRetornarImoveisDoCliente()
    {
        var clienteId = Guid.NewGuid();
        var favoritos = new List<Favorito>
        {
            new Favorito { ClienteId = clienteId, Imovel = new Imovel { Titulo = "Imóvel 1", Codigo = "IM-1", Endereco = new Endereco { Bairro = "B", Cidade = "C", Estado = "PR" } } },
            new Favorito { ClienteId = clienteId, Imovel = new Imovel { Titulo = "Imóvel 2", Codigo = "IM-2", Endereco = new Endereco { Bairro = "B", Cidade = "C", Estado = "PR" } } }
        };
        _favRepoMock.Setup(r => r.ObterPorClienteAsync(clienteId)).ReturnsAsync(favoritos);

        var resultado = await _service.ObterFavoritosAsync(clienteId);

        resultado.Should().HaveCount(2);
    }
}
