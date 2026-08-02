using FluentAssertions;
using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Services;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Enums;
using Imobiliaria.Domain.Interfaces;
using Moq;
using Xunit;

namespace Imobiliaria.Tests.Services;

public class ContratoServiceTests
{
    private readonly Mock<IContratoRepository> _contratoRepoMock;
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly ContratoService _service;

    public ContratoServiceTests()
    {
        _contratoRepoMock = new Mock<IContratoRepository>();
        _imovelRepoMock = new Mock<IImovelRepository>();
        _service = new ContratoService(_contratoRepoMock.Object, _imovelRepoMock.Object);
    }

    private Contrato CriarContratoFake(StatusContrato status = StatusContrato.Ativo)
    {
        return new Contrato
        {
            Id = Guid.NewGuid(), Codigo = "CTR-TEST",
            ImovelId = Guid.NewGuid(), ClienteId = Guid.NewGuid(),
            Tipo = FinalidadeImovel.Locacao, Status = status,
            DataInicio = DateTime.UtcNow.AddMonths(-6),
            DataFim = DateTime.UtcNow.AddMonths(6),
            ValorTotal = 26400m, ValorMensal = 2200m,
            Imovel = new Imovel { Titulo = "Apto Teste", Codigo = "APT-001" },
            Cliente = new Cliente { Nome = "Cliente Teste" },
            Corretor = new Corretor { Nome = "Corretor Teste", CRECI = "CRECI-00" }
        };
    }

    // ===== CriarAsync =====

    [Fact]
    public async Task CriarAsync_Locacao_DeveAlterarStatusImovelParaAlugado()
    {
        var imovelId = Guid.NewGuid();
        var imovel = new Imovel { Id = imovelId, Status = StatusImovel.Disponivel };
        _imovelRepoMock.Setup(r => r.ObterPorIdAsync(imovelId)).ReturnsAsync(imovel);
        _contratoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Contrato>())).ReturnsAsync((Contrato c) => c);
        _contratoRepoMock.Setup(r => r.ObterComDetalhesAsync(It.IsAny<Guid>())).ReturnsAsync(CriarContratoFake());

        var dto = new ContratoCreateDto
        {
            ImovelId = imovelId, ClienteId = Guid.NewGuid(),
            Tipo = FinalidadeImovel.Locacao, DataInicio = DateTime.UtcNow,
            ValorTotal = 26400m, ValorMensal = 2200m
        };

        await _service.CriarAsync(dto);

        imovel.Status.Should().Be(StatusImovel.Alugado);
        _imovelRepoMock.Verify(r => r.AtualizarAsync(imovel), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_Venda_DeveAlterarStatusImovelParaVendido()
    {
        var imovelId = Guid.NewGuid();
        var imovel = new Imovel { Id = imovelId, Status = StatusImovel.Disponivel };
        _imovelRepoMock.Setup(r => r.ObterPorIdAsync(imovelId)).ReturnsAsync(imovel);
        _contratoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Contrato>())).ReturnsAsync((Contrato c) => c);
        _contratoRepoMock.Setup(r => r.ObterComDetalhesAsync(It.IsAny<Guid>())).ReturnsAsync(CriarContratoFake());

        var dto = new ContratoCreateDto
        {
            ImovelId = imovelId, ClienteId = Guid.NewGuid(),
            Tipo = FinalidadeImovel.Venda, DataInicio = DateTime.UtcNow, ValorTotal = 500000m
        };

        await _service.CriarAsync(dto);

        imovel.Status.Should().Be(StatusImovel.Vendido);
    }

    [Fact]
    public async Task CriarAsync_DeveGerarCodigoComPrefixoCTR()
    {
        Contrato? salvo = null;
        _contratoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Contrato>()))
            .Callback<Contrato>(c => salvo = c).ReturnsAsync((Contrato c) => c);
        _contratoRepoMock.Setup(r => r.ObterComDetalhesAsync(It.IsAny<Guid>())).ReturnsAsync(CriarContratoFake());
        _imovelRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Imovel());

        await _service.CriarAsync(new ContratoCreateDto
        {
            ImovelId = Guid.NewGuid(), ClienteId = Guid.NewGuid(),
            Tipo = FinalidadeImovel.Locacao, DataInicio = DateTime.UtcNow, ValorTotal = 10000m
        });

        salvo.Should().NotBeNull();
        salvo!.Codigo.Should().StartWith("CTR-");
    }

    // ===== RescindirAsync =====

    [Fact]
    public async Task RescindirAsync_DeveAlterarStatusParaRescindidoEImovelParaEmAnalise()
    {
        var contrato = CriarContratoFake();
        var imovel = new Imovel { Id = contrato.ImovelId, Status = StatusImovel.Alugado };

        _contratoRepoMock.Setup(r => r.ObterPorIdAsync(contrato.Id)).ReturnsAsync(contrato);
        _imovelRepoMock.Setup(r => r.ObterPorIdAsync(contrato.ImovelId)).ReturnsAsync(imovel);
        _contratoRepoMock.Setup(r => r.ObterComDetalhesAsync(contrato.Id)).ReturnsAsync(contrato);

        var resultado = await _service.RescindirAsync(contrato.Id, "Teste rescisão");

        resultado.Should().NotBeNull();
        contrato.Status.Should().Be(StatusContrato.Rescindido);
        contrato.DataRescisao.Should().NotBeNull();
        imovel.Status.Should().Be(StatusImovel.EmAnalise);
    }

    [Fact]
    public async Task RescindirAsync_IdInexistente_DeveRetornarNull()
    {
        _contratoRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Contrato?)null);

        var resultado = await _service.RescindirAsync(Guid.NewGuid(), "motivo");

        resultado.Should().BeNull();
    }

    // ===== RenovarAsync =====

    [Fact]
    public async Task RenovarAsync_DeveCriarNovoContratoEMarcarAntigoComoRenovado()
    {
        var contrato = CriarContratoFake();
        _contratoRepoMock.Setup(r => r.ObterPorIdAsync(contrato.Id)).ReturnsAsync(contrato);

        Contrato? novoContrato = null;
        _contratoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Contrato>()))
            .Callback<Contrato>(c => novoContrato = c).ReturnsAsync((Contrato c) => c);
        _contratoRepoMock.Setup(r => r.ObterComDetalhesAsync(It.IsAny<Guid>())).ReturnsAsync(CriarContratoFake());

        var novaData = DateTime.UtcNow.AddYears(1);
        await _service.RenovarAsync(contrato.Id, novaData, 2500m);

        contrato.Status.Should().Be(StatusContrato.Renovado);
        novoContrato.Should().NotBeNull();
        novoContrato!.DataFim.Should().Be(novaData);
        novoContrato.ValorTotal.Should().Be(2500m);
        novoContrato.Codigo.Should().StartWith("CTR-");
    }

    // ===== AtualizarAsync =====

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarCamposInformados()
    {
        var contrato = CriarContratoFake();
        _contratoRepoMock.Setup(r => r.ObterPorIdAsync(contrato.Id)).ReturnsAsync(contrato);
        _contratoRepoMock.Setup(r => r.ObterComDetalhesAsync(contrato.Id)).ReturnsAsync(contrato);

        var dto = new ContratoUpdateDto { ValorMensal = 3000m, Observacoes = "Reajuste" };
        await _service.AtualizarAsync(contrato.Id, dto);

        contrato.ValorMensal.Should().Be(3000m);
        contrato.Observacoes.Should().Be("Reajuste");
    }
}
