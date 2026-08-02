using FluentAssertions;
using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Services;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Interfaces;
using Moq;
using Xunit;

namespace Imobiliaria.Tests.Services;

public class FinanceiroServiceTests
{
    private readonly Mock<ILancamentoRepository> _lancamentoRepoMock;
    private readonly Mock<IComissaoRepository> _comissaoRepoMock;
    private readonly FinanceiroService _service;

    public FinanceiroServiceTests()
    {
        _lancamentoRepoMock = new Mock<ILancamentoRepository>();
        _comissaoRepoMock = new Mock<IComissaoRepository>();
        _service = new FinanceiroService(_lancamentoRepoMock.Object, _comissaoRepoMock.Object);
    }

    [Fact]
    public async Task ObterResumoAsync_DeveCalcularSaldoCorretamente()
    {
        var lancamentos = new List<Lancamento>
        {
            new Lancamento { Tipo = TipoLancamento.Receita, Valor = 5000m, Pago = true, Ativo = true, DataVencimento = DateTime.UtcNow },
            new Lancamento { Tipo = TipoLancamento.Receita, Valor = 3000m, Pago = true, Ativo = true, DataVencimento = DateTime.UtcNow },
            new Lancamento { Tipo = TipoLancamento.Despesa, Valor = 2000m, Pago = true, Ativo = true, DataVencimento = DateTime.UtcNow },
            new Lancamento { Tipo = TipoLancamento.Receita, Valor = 1000m, Pago = false, Ativo = true, DataVencimento = DateTime.UtcNow.AddDays(5) },
        };
        _lancamentoRepoMock.Setup(r => r.ObterComDetalhesAsync()).ReturnsAsync(lancamentos);
        _comissaoRepoMock.Setup(r => r.ObterPendentesAsync()).ReturnsAsync(new List<Comissao>());

        var resumo = await _service.ObterResumoAsync();

        resumo.ReceitaTotal.Should().Be(8000m);
        resumo.DespesaTotal.Should().Be(2000m);
        resumo.Saldo.Should().Be(6000m);
        resumo.ReceitaPendente.Should().Be(1000m);
    }

    [Fact]
    public async Task ObterResumoAsync_DeveContarVencidos()
    {
        var lancamentos = new List<Lancamento>
        {
            new Lancamento { Tipo = TipoLancamento.Receita, Valor = 100m, Pago = false, Ativo = true, DataVencimento = DateTime.UtcNow.AddDays(-10) },
            new Lancamento { Tipo = TipoLancamento.Despesa, Valor = 200m, Pago = false, Ativo = true, DataVencimento = DateTime.UtcNow.AddDays(-5) },
            new Lancamento { Tipo = TipoLancamento.Receita, Valor = 300m, Pago = false, Ativo = true, DataVencimento = DateTime.UtcNow.AddDays(10) },
        };
        _lancamentoRepoMock.Setup(r => r.ObterComDetalhesAsync()).ReturnsAsync(lancamentos);
        _comissaoRepoMock.Setup(r => r.ObterPendentesAsync()).ReturnsAsync(new List<Comissao>());

        var resumo = await _service.ObterResumoAsync();

        resumo.LancamentosVencidos.Should().Be(2);
    }

    [Fact]
    public async Task CriarLancamentoAsync_DeveRetornarDtoCorreto()
    {
        _lancamentoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Lancamento>())).ReturnsAsync((Lancamento l) => l);

        var dto = new LancamentoCreateDto
        {
            Descricao = "Aluguel Teste", Tipo = TipoLancamento.Receita,
            Categoria = CategoriaLancamento.Aluguel, Valor = 2200m,
            DataVencimento = DateTime.UtcNow.AddDays(10)
        };

        var resultado = await _service.CriarLancamentoAsync(dto);

        resultado.Should().NotBeNull();
        resultado.Descricao.Should().Be("Aluguel Teste");
        resultado.Valor.Should().Be(2200m);
        resultado.Pago.Should().BeFalse();
    }

    [Fact]
    public async Task PagarLancamentoAsync_DeveMarcarComoPago()
    {
        var lancamento = new Lancamento { Id = Guid.NewGuid(), Descricao = "Teste", Valor = 100m, Pago = false };
        _lancamentoRepoMock.Setup(r => r.ObterPorIdAsync(lancamento.Id)).ReturnsAsync(lancamento);

        var resultado = await _service.PagarLancamentoAsync(lancamento.Id);

        resultado.Should().NotBeNull();
        lancamento.Pago.Should().BeTrue();
        lancamento.DataPagamento.Should().NotBeNull();
    }

    [Fact]
    public async Task CriarComissaoAsync_DeveCalcularValorCorreto()
    {
        _comissaoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Comissao>())).ReturnsAsync((Comissao c) => c);

        var dto = new ComissaoCreateDto
        {
            CorretorId = Guid.NewGuid(), ValorBase = 500000m, Percentual = 5m
        };

        var resultado = await _service.CriarComissaoAsync(dto);

        resultado.Should().NotBeNull();
        resultado.ValorComissao.Should().Be(25000m); // 500000 * 5 / 100
    }

    [Fact]
    public async Task PagarComissaoAsync_DeveMarcarComoPaga()
    {
        var comissao = new Comissao { Id = Guid.NewGuid(), Pago = false, ValorComissao = 5000m };
        _comissaoRepoMock.Setup(r => r.ObterPorIdAsync(comissao.Id)).ReturnsAsync(comissao);

        var resultado = await _service.PagarComissaoAsync(comissao.Id);

        resultado.Should().NotBeNull();
        comissao.Pago.Should().BeTrue();
        comissao.DataPagamento.Should().NotBeNull();
    }

    [Fact]
    public async Task PagarLancamentoAsync_Inexistente_DeveRetornarNull()
    {
        _lancamentoRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Lancamento?)null);

        var resultado = await _service.PagarLancamentoAsync(Guid.NewGuid());

        resultado.Should().BeNull();
    }
}
