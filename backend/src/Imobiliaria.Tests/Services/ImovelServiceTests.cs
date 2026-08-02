using System.Linq.Expressions;
using FluentAssertions;
using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Services;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Enums;
using Imobiliaria.Domain.Interfaces;
using Moq;
using Xunit;

namespace Imobiliaria.Tests.Services;

public class ImovelServiceTests
{
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly Mock<IClienteRepository> _clienteRepoMock;
    private readonly Mock<ICorretorRepository> _corretorRepoMock;
    private readonly Mock<IProprietarioRepository> _proprietarioRepoMock;
    private readonly ImovelService _service;

    public ImovelServiceTests()
    {
        _imovelRepoMock = new Mock<IImovelRepository>();
        _clienteRepoMock = new Mock<IClienteRepository>();
        _corretorRepoMock = new Mock<ICorretorRepository>();
        _proprietarioRepoMock = new Mock<IProprietarioRepository>();
        _service = new ImovelService(
            _imovelRepoMock.Object,
            _clienteRepoMock.Object,
            _corretorRepoMock.Object,
            _proprietarioRepoMock.Object);
    }

    private Imovel CriarImovelFake(
        TipoImovel tipo = TipoImovel.Casa,
        FinalidadeImovel finalidade = FinalidadeImovel.Venda,
        StatusImovel status = StatusImovel.Disponivel)
    {
        return new Imovel
        {
            Id = Guid.NewGuid(),
            Titulo = "Casa Teste",
            Codigo = "CAS-TEST",
            Tipo = tipo,
            Finalidade = finalidade,
            Status = status,
            PrecoVenda = 500000m,
            AreaTotal = 200,
            Dormitorios = 3,
            Banheiros = 2,
            VagasGaragem = 2,
            Endereco = new Endereco
            {
                Logradouro = "Rua Teste",
                Numero = "100",
                Bairro = "Centro",
                Cidade = "Maringá",
                Estado = "PR",
                CEP = "87000-000"
            },
            Proprietario = new Proprietario { Nome = "Proprietário Teste", CPFouCNPJ = "000.000.000-00" },
            Corretor = new Corretor { Nome = "Corretor Teste", CRECI = "CRECI-00000" },
            ProprietarioId = Guid.NewGuid(),
            CorretorId = Guid.NewGuid()
        };
    }

    // ===== ObterTodosAsync =====

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaDeImoveis()
    {
        // Arrange
        var imoveis = new List<Imovel> { CriarImovelFake(), CriarImovelFake() };
        _imovelRepoMock.Setup(r => r.ObterComDetalhesAsync()).ReturnsAsync(imoveis);

        // Act
        var resultado = await _service.ObterTodosAsync();

        // Assert
        resultado.Should().HaveCount(2);
        _imovelRepoMock.Verify(r => r.ObterComDetalhesAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterTodosAsync_ListaVazia_DeveRetornarVazio()
    {
        _imovelRepoMock.Setup(r => r.ObterComDetalhesAsync()).ReturnsAsync(new List<Imovel>());

        var resultado = await _service.ObterTodosAsync();

        resultado.Should().BeEmpty();
    }

    // ===== ObterPorIdAsync =====

    [Fact]
    public async Task ObterPorIdAsync_IdExistente_DeveRetornarImovel()
    {
        var imovel = CriarImovelFake();
        _imovelRepoMock.Setup(r => r.ObterComDetalhesPorIdAsync(imovel.Id)).ReturnsAsync(imovel);

        var resultado = await _service.ObterPorIdAsync(imovel.Id);

        resultado.Should().NotBeNull();
        resultado!.Titulo.Should().Be("Casa Teste");
        resultado.Tipo.Should().Be("Casa");
    }

    [Fact]
    public async Task ObterPorIdAsync_IdInexistente_DeveRetornarNull()
    {
        _imovelRepoMock.Setup(r => r.ObterComDetalhesPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Imovel?)null);

        var resultado = await _service.ObterPorIdAsync(Guid.NewGuid());

        resultado.Should().BeNull();
    }

    // ===== CriarAsync =====

    [Fact]
    public async Task CriarAsync_DadosValidos_DeveCriarERetornarDto()
    {
        var dto = new ImovelCreateDto
        {
            Titulo = "Novo Imóvel",
            Tipo = TipoImovel.Apartamento,
            Finalidade = FinalidadeImovel.Locacao,
            PrecoLocacao = 2000m,
            AreaTotal = 80,
            Dormitorios = 2,
            Banheiros = 1,
            VagasGaragem = 1,
            ProprietarioId = Guid.NewGuid(),
            Endereco = new EnderecoCreateDto
            {
                Logradouro = "Av Brasil", Numero = "500", Bairro = "Zona 7",
                Cidade = "Maringá", Estado = "PR", CEP = "87020-000"
            }
        };

        _imovelRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Imovel>()))
            .ReturnsAsync((Imovel i) => i);

        var imovelCriado = CriarImovelFake(TipoImovel.Apartamento, FinalidadeImovel.Locacao);
        imovelCriado.Titulo = "Novo Imóvel";
        _imovelRepoMock.Setup(r => r.ObterComDetalhesPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(imovelCriado);

        var resultado = await _service.CriarAsync(dto);

        resultado.Should().NotBeNull();
        resultado.Titulo.Should().Be("Novo Imóvel");
        _imovelRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Imovel>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveGerarCodigoComPrefixoCorreto()
    {
        var dto = new ImovelCreateDto
        {
            Titulo = "Terreno",
            Tipo = TipoImovel.Terreno,
            Finalidade = FinalidadeImovel.Venda,
            AreaTotal = 300,
            ProprietarioId = Guid.NewGuid(),
            Endereco = new EnderecoCreateDto
            {
                Logradouro = "Rua X", Numero = "1", Bairro = "B",
                Cidade = "C", Estado = "PR", CEP = "00000-000"
            }
        };

        Imovel? imovelSalvo = null;
        _imovelRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Imovel>()))
            .Callback<Imovel>(i => imovelSalvo = i)
            .ReturnsAsync((Imovel i) => i);
        _imovelRepoMock.Setup(r => r.ObterComDetalhesPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(CriarImovelFake(TipoImovel.Terreno));

        await _service.CriarAsync(dto);

        imovelSalvo.Should().NotBeNull();
        imovelSalvo!.Codigo.Should().StartWith("TER-");
    }

    // ===== AtualizarAsync =====

    [Fact]
    public async Task AtualizarAsync_IdExistente_DeveAtualizarCampos()
    {
        var imovel = CriarImovelFake();
        _imovelRepoMock.Setup(r => r.ObterPorIdAsync(imovel.Id)).ReturnsAsync(imovel);
        _imovelRepoMock.Setup(r => r.ObterComDetalhesPorIdAsync(imovel.Id)).ReturnsAsync(imovel);

        var dto = new ImovelUpdateDto { Titulo = "Título Atualizado", PrecoVenda = 600000m };

        var resultado = await _service.AtualizarAsync(imovel.Id, dto);

        resultado.Should().NotBeNull();
        imovel.Titulo.Should().Be("Título Atualizado");
        imovel.PrecoVenda.Should().Be(600000m);
        imovel.AtualizadoEm.Should().NotBeNull();
        _imovelRepoMock.Verify(r => r.AtualizarAsync(It.IsAny<Imovel>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_IdInexistente_DeveRetornarNull()
    {
        _imovelRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Imovel?)null);

        var resultado = await _service.AtualizarAsync(Guid.NewGuid(), new ImovelUpdateDto { Titulo = "X" });

        resultado.Should().BeNull();
    }

    // ===== RemoverAsync =====

    [Fact]
    public async Task RemoverAsync_IdExistente_DeveDesativarERetornarTrue()
    {
        var imovel = CriarImovelFake();
        _imovelRepoMock.Setup(r => r.ObterPorIdAsync(imovel.Id)).ReturnsAsync(imovel);

        var resultado = await _service.RemoverAsync(imovel.Id);

        resultado.Should().BeTrue();
        imovel.Ativo.Should().BeFalse();
        _imovelRepoMock.Verify(r => r.AtualizarAsync(imovel), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_IdInexistente_DeveRetornarFalse()
    {
        _imovelRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Imovel?)null);

        var resultado = await _service.RemoverAsync(Guid.NewGuid());

        resultado.Should().BeFalse();
    }

    // ===== FiltrarAsync =====

    [Fact]
    public async Task FiltrarAsync_ComCidade_DevePassarParametroCorreto()
    {
        var imoveis = new List<Imovel> { CriarImovelFake() };
        _imovelRepoMock.Setup(r => r.FiltrarAsync("Maringá", null, null, null, null, null, null, null, null))
            .ReturnsAsync(imoveis);

        var filtro = new ImovelFilterDto { Cidade = "Maringá" };
        var resultado = await _service.FiltrarAsync(filtro);

        resultado.Should().HaveCount(1);
    }

    // ===== ObterDashboardAsync =====

    [Fact]
    public async Task ObterDashboardAsync_DeveRetornarIndicadoresCorretos()
    {
        _imovelRepoMock.Setup(r => r.ContarAsync(It.IsAny<Expression<Func<Imovel, bool>>>()))
            .ReturnsAsync(5);
        _clienteRepoMock.Setup(r => r.ContarAsync(It.IsAny<Expression<Func<Cliente, bool>>>()))
            .ReturnsAsync(10);
        _corretorRepoMock.Setup(r => r.ContarAsync(It.IsAny<Expression<Func<Corretor, bool>>>()))
            .ReturnsAsync(3);
        _proprietarioRepoMock.Setup(r => r.ContarAsync(It.IsAny<Expression<Func<Proprietario, bool>>>()))
            .ReturnsAsync(8);
        _imovelRepoMock.Setup(r => r.ObterComDetalhesAsync()).ReturnsAsync(new List<Imovel>());

        var resultado = await _service.ObterDashboardAsync();

        resultado.Should().NotBeNull();
        resultado.TotalClientes.Should().Be(10);
        resultado.TotalCorretores.Should().Be(3);
        resultado.TotalProprietarios.Should().Be(8);
    }

    // ===== Mapeamento DTO =====

    [Fact]
    public async Task ObterPorIdAsync_DeveMapearTodosCamposDoDto()
    {
        var imovel = CriarImovelFake();
        imovel.Suites = 2;
        imovel.Mobiliado = true;
        imovel.ValorCondominio = 500m;
        imovel.ValorIPTU = 1200m;
        imovel.CaracteristicasJson = "[\"Piscina\",\"Churrasqueira\"]";

        _imovelRepoMock.Setup(r => r.ObterComDetalhesPorIdAsync(imovel.Id)).ReturnsAsync(imovel);

        var dto = await _service.ObterPorIdAsync(imovel.Id);

        dto.Should().NotBeNull();
        dto!.Id.Should().Be(imovel.Id);
        dto.PrecoVenda.Should().Be(500000m);
        dto.AreaTotal.Should().Be(200);
        dto.Dormitorios.Should().Be(3);
        dto.Suites.Should().Be(2);
        dto.Mobiliado.Should().BeTrue();
        dto.ValorCondominio.Should().Be(500m);
        dto.Caracteristicas.Should().Contain("Piscina");
        dto.Endereco.Should().NotBeNull();
        dto.Endereco!.Cidade.Should().Be("Maringá");
        dto.ProprietarioNome.Should().Be("Proprietário Teste");
        dto.CorretorNome.Should().Be("Corretor Teste");
    }
}
