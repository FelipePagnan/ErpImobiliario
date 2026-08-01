using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Interfaces;

namespace Imobiliaria.Application.Services;

public class FinanceiroService : IFinanceiroService
{
    private readonly ILancamentoRepository _lancamentoRepo;
    private readonly IComissaoRepository _comissaoRepo;

    public FinanceiroService(ILancamentoRepository lancamentoRepo, IComissaoRepository comissaoRepo)
    {
        _lancamentoRepo = lancamentoRepo;
        _comissaoRepo = comissaoRepo;
    }

    public async Task<ResumoFinanceiroDto> ObterResumoAsync()
    {
        var todos = (await _lancamentoRepo.ObterComDetalhesAsync()).Where(l => l.Ativo).ToList();
        var comissoesPend = await _comissaoRepo.ObterPendentesAsync();

        var receitas = todos.Where(l => l.Tipo == TipoLancamento.Receita);
        var despesas = todos.Where(l => l.Tipo == TipoLancamento.Despesa);

        return new ResumoFinanceiroDto
        {
            ReceitaTotal = receitas.Where(l => l.Pago).Sum(l => l.Valor),
            DespesaTotal = despesas.Where(l => l.Pago).Sum(l => l.Valor),
            Saldo = receitas.Where(l => l.Pago).Sum(l => l.Valor) - despesas.Where(l => l.Pago).Sum(l => l.Valor),
            ReceitaPendente = receitas.Where(l => !l.Pago).Sum(l => l.Valor),
            DespesaPendente = despesas.Where(l => !l.Pago).Sum(l => l.Valor),
            ComissoesPendentes = comissoesPend.Sum(c => c.ValorComissao),
            LancamentosVencidos = todos.Count(l => !l.Pago && l.DataVencimento < DateTime.UtcNow),
            UltimosLancamentos = todos.OrderByDescending(l => l.CriadoEm).Take(10).Select(MapLancamentoToDto).ToList()
        };
    }

    public async Task<IEnumerable<LancamentoDto>> ObterLancamentosAsync()
    {
        var lancamentos = await _lancamentoRepo.ObterComDetalhesAsync();
        return lancamentos.Where(l => l.Ativo).OrderByDescending(l => l.DataVencimento).Select(MapLancamentoToDto);
    }

    public async Task<IEnumerable<LancamentoDto>> ObterPorPeriodoAsync(DateTime inicio, DateTime fim)
    {
        var lancamentos = await _lancamentoRepo.ObterPorPeriodoAsync(inicio, fim);
        return lancamentos.Select(MapLancamentoToDto);
    }

    public async Task<LancamentoDto> CriarLancamentoAsync(LancamentoCreateDto dto)
    {
        var lancamento = new Lancamento
        {
            Descricao = dto.Descricao, Tipo = dto.Tipo, Categoria = dto.Categoria,
            Valor = dto.Valor, DataVencimento = dto.DataVencimento,
            ContratoId = dto.ContratoId, ImovelId = dto.ImovelId, Observacoes = dto.Observacoes
        };
        await _lancamentoRepo.AdicionarAsync(lancamento);
        return MapLancamentoToDto(lancamento);
    }

    public async Task<LancamentoDto?> AtualizarLancamentoAsync(Guid id, LancamentoUpdateDto dto)
    {
        var l = await _lancamentoRepo.ObterPorIdAsync(id);
        if (l == null) return null;

        if (dto.Descricao != null) l.Descricao = dto.Descricao;
        if (dto.Valor.HasValue) l.Valor = dto.Valor.Value;
        if (dto.DataVencimento.HasValue) l.DataVencimento = dto.DataVencimento.Value;
        if (dto.DataPagamento.HasValue) l.DataPagamento = dto.DataPagamento;
        if (dto.Pago.HasValue) { l.Pago = dto.Pago.Value; if (l.Pago && !l.DataPagamento.HasValue) l.DataPagamento = DateTime.UtcNow; }
        if (dto.Observacoes != null) l.Observacoes = dto.Observacoes;
        l.AtualizadoEm = DateTime.UtcNow;

        await _lancamentoRepo.AtualizarAsync(l);
        return MapLancamentoToDto(l);
    }

    public async Task<LancamentoDto?> PagarLancamentoAsync(Guid id)
    {
        var l = await _lancamentoRepo.ObterPorIdAsync(id);
        if (l == null) return null;
        l.Pago = true;
        l.DataPagamento = DateTime.UtcNow;
        l.AtualizadoEm = DateTime.UtcNow;
        await _lancamentoRepo.AtualizarAsync(l);
        return MapLancamentoToDto(l);
    }

    public async Task<IEnumerable<ComissaoDto>> ObterComissoesAsync()
    {
        var comissoes = await _comissaoRepo.ObterComDetalhesAsync();
        return comissoes.Where(c => c.Ativo).OrderByDescending(c => c.DataCalculo).Select(MapComissaoToDto);
    }

    public async Task<ComissaoDto> CriarComissaoAsync(ComissaoCreateDto dto)
    {
        var comissao = new Comissao
        {
            CorretorId = dto.CorretorId, ContratoId = dto.ContratoId, ImovelId = dto.ImovelId,
            ValorBase = dto.ValorBase, Percentual = dto.Percentual,
            ValorComissao = dto.ValorBase * (dto.Percentual / 100),
            Observacoes = dto.Observacoes
        };
        await _comissaoRepo.AdicionarAsync(comissao);
        return MapComissaoToDto(comissao);
    }

    public async Task<ComissaoDto?> PagarComissaoAsync(Guid id)
    {
        var c = await _comissaoRepo.ObterPorIdAsync(id);
        if (c == null) return null;
        c.Pago = true;
        c.DataPagamento = DateTime.UtcNow;
        c.AtualizadoEm = DateTime.UtcNow;
        await _comissaoRepo.AtualizarAsync(c);
        return MapComissaoToDto(c);
    }

    private static LancamentoDto MapLancamentoToDto(Lancamento l) => new()
    {
        Id = l.Id, Descricao = l.Descricao,
        Tipo = l.Tipo.ToString(), TipoId = (int)l.Tipo,
        Categoria = l.Categoria.ToString(), CategoriaId = (int)l.Categoria,
        Valor = l.Valor, DataVencimento = l.DataVencimento,
        DataPagamento = l.DataPagamento, Pago = l.Pago,
        ContratoId = l.ContratoId, ContratoCodigo = l.Contrato?.Codigo,
        ImovelId = l.ImovelId, ImovelTitulo = l.Imovel?.Titulo,
        Observacoes = l.Observacoes, CriadoEm = l.CriadoEm
    };

    private static ComissaoDto MapComissaoToDto(Comissao c) => new()
    {
        Id = c.Id, CorretorId = c.CorretorId, CorretorNome = c.Corretor?.Nome ?? "",
        ContratoId = c.ContratoId, ContratoCodigo = c.Contrato?.Codigo,
        ImovelId = c.ImovelId, ImovelTitulo = c.Imovel?.Titulo,
        ValorBase = c.ValorBase, Percentual = c.Percentual, ValorComissao = c.ValorComissao,
        DataCalculo = c.DataCalculo, Pago = c.Pago, DataPagamento = c.DataPagamento,
        Observacoes = c.Observacoes
    };
}
