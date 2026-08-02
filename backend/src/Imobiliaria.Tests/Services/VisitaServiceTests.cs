using FluentAssertions;
using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Services;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Enums;
using Imobiliaria.Domain.Interfaces;
using Moq;
using Xunit;

namespace Imobiliaria.Tests.Services;

public class VisitaServiceTests
{
    private readonly Mock<IVisitaRepository> _visitaRepoMock;
    private readonly Mock<IImovelRepository> _imovelRepoMock;
    private readonly VisitaService _service;

    public VisitaServiceTests()
    {
        _visitaRepoMock = new Mock<IVisitaRepository>();
        _imovelRepoMock = new Mock<IImovelRepository>();
        _service = new VisitaService(_visitaRepoMock.Object, _imovelRepoMock.Object);
    }

    private Visita CriarVisitaFake(StatusVisita status = StatusVisita.Solicitada)
    {
        return new Visita
        {
            Id = Guid.NewGuid(), ImovelId = Guid.NewGuid(), ClienteId = Guid.NewGuid(),
            Status = status, DataSolicitacao = DateTime.UtcNow,
            Imovel = new Imovel { Titulo = "Imóvel Teste", Codigo = "IMV-001" },
            Cliente = new Cliente { Nome = "Cliente Teste", Telefone = "(44) 99999-0000" },
            Corretor = new Corretor { Nome = "Corretor Teste", CRECI = "CRECI-00" }
        };
    }

    [Fact]
    public async Task CriarAsync_SemData_DeveStatusSolicitada()
    {
        var imovel = new Imovel { Id = Guid.NewGuid(), CorretorId = Guid.NewGuid() };
        _imovelRepoMock.Setup(r => r.ObterComDetalhesPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(imovel);
        _visitaRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Visita>())).ReturnsAsync((Visita v) => v);
        _visitaRepoMock.Setup(r => r.ObterComDetalhesAsync(It.IsAny<Guid>())).ReturnsAsync(CriarVisitaFake());

        var dto = new VisitaCreateDto { ImovelId = imovel.Id, ClienteId = Guid.NewGuid() };
        var resultado = await _service.CriarAsync(dto);

        resultado.Should().NotBeNull();
        resultado.Status.Should().Be("Solicitada");
    }

    [Fact]
    public async Task CriarAsync_ComData_DeveStatusAgendada()
    {
        var imovel = new Imovel { Id = Guid.NewGuid(), CorretorId = Guid.NewGuid() };
        _imovelRepoMock.Setup(r => r.ObterComDetalhesPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(imovel);

        Visita? salva = null;
        _visitaRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Visita>()))
            .Callback<Visita>(v => salva = v).ReturnsAsync((Visita v) => v);
        _visitaRepoMock.Setup(r => r.ObterComDetalhesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(CriarVisitaFake(StatusVisita.Agendada));

        var dto = new VisitaCreateDto
        {
            ImovelId = imovel.Id, ClienteId = Guid.NewGuid(),
            DataAgendada = DateTime.UtcNow.AddDays(3)
        };

        await _service.CriarAsync(dto);

        salva.Should().NotBeNull();
        salva!.Status.Should().Be(StatusVisita.Agendada);
    }

    [Fact]
    public async Task CriarAsync_DeveVincularCorretorDoImovel()
    {
        var corretorId = Guid.NewGuid();
        var imovel = new Imovel { Id = Guid.NewGuid(), CorretorId = corretorId };
        _imovelRepoMock.Setup(r => r.ObterComDetalhesPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(imovel);

        Visita? salva = null;
        _visitaRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Visita>()))
            .Callback<Visita>(v => salva = v).ReturnsAsync((Visita v) => v);
        _visitaRepoMock.Setup(r => r.ObterComDetalhesAsync(It.IsAny<Guid>())).ReturnsAsync(CriarVisitaFake());

        await _service.CriarAsync(new VisitaCreateDto { ImovelId = imovel.Id, ClienteId = Guid.NewGuid() });

        salva!.CorretorId.Should().Be(corretorId);
    }

    [Fact]
    public async Task CancelarAsync_DeveAlterarStatusParaCancelada()
    {
        var visita = CriarVisitaFake();
        _visitaRepoMock.Setup(r => r.ObterPorIdAsync(visita.Id)).ReturnsAsync(visita);

        var resultado = await _service.CancelarAsync(visita.Id);

        resultado.Should().BeTrue();
        visita.Status.Should().Be(StatusVisita.Cancelada);
    }

    [Fact]
    public async Task CancelarAsync_IdInexistente_DeveRetornarFalse()
    {
        _visitaRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Visita?)null);

        var resultado = await _service.CancelarAsync(Guid.NewGuid());

        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarStatusEObservacoes()
    {
        var visita = CriarVisitaFake();
        _visitaRepoMock.Setup(r => r.ObterPorIdAsync(visita.Id)).ReturnsAsync(visita);
        _visitaRepoMock.Setup(r => r.ObterComDetalhesAsync(visita.Id)).ReturnsAsync(visita);

        var dto = new VisitaUpdateDto { Status = StatusVisita.Realizada, FeedbackCliente = "Gostou muito" };
        var resultado = await _service.AtualizarAsync(visita.Id, dto);

        resultado.Should().NotBeNull();
        visita.Status.Should().Be(StatusVisita.Realizada);
        visita.FeedbackCliente.Should().Be("Gostou muito");
    }
}
