using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Enums;
using Imobiliaria.Domain.Interfaces;

namespace Imobiliaria.Application.Services;

public class ContratoService : IContratoService
{
    private readonly IContratoRepository _contratoRepo;
    private readonly IImovelRepository _imovelRepo;

    public ContratoService(IContratoRepository contratoRepo, IImovelRepository imovelRepo)
    {
        _contratoRepo = contratoRepo;
        _imovelRepo = imovelRepo;
    }

    public async Task<IEnumerable<ContratoDto>> ObterTodosAsync()
    {
        var contratos = await _contratoRepo.ObterComDetalhesAsync();
        return contratos.Select(MapToDto);
    }

    public async Task<ContratoDto?> ObterPorIdAsync(Guid id)
    {
        var c = await _contratoRepo.ObterComDetalhesAsync(id);
        return c == null ? null : MapToDto(c);
    }

    public async Task<IEnumerable<ContratoDto>> ObterVencendoAsync(int dias = 30)
    {
        var contratos = await _contratoRepo.ObterVencendoAsync(dias);
        return contratos.Select(MapToDto);
    }

    public async Task<ContratoDto> CriarAsync(ContratoCreateDto dto)
    {
        var contrato = new Contrato
        {
            Codigo = $"CTR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            ImovelId = dto.ImovelId,
            ClienteId = dto.ClienteId,
            CorretorId = dto.CorretorId,
            Tipo = dto.Tipo,
            DataInicio = dto.DataInicio,
            DataFim = dto.DataFim,
            ValorTotal = dto.ValorTotal,
            ValorMensal = dto.ValorMensal,
            MultaRescisao = dto.MultaRescisao,
            Observacoes = dto.Observacoes
        };

        await _contratoRepo.AdicionarAsync(contrato);

        // Atualiza status do imóvel
        var imovel = await _imovelRepo.ObterPorIdAsync(dto.ImovelId);
        if (imovel != null)
        {
            imovel.Status = dto.Tipo == FinalidadeImovel.Locacao ? StatusImovel.Alugado : StatusImovel.Vendido;
            await _imovelRepo.AtualizarAsync(imovel);
        }

        var full = await _contratoRepo.ObterComDetalhesAsync(contrato.Id);
        return MapToDto(full!);
    }

    public async Task<ContratoDto?> AtualizarAsync(Guid id, ContratoUpdateDto dto)
    {
        var contrato = await _contratoRepo.ObterPorIdAsync(id);
        if (contrato == null) return null;

        if (dto.Status.HasValue) contrato.Status = dto.Status.Value;
        if (dto.DataFim.HasValue) contrato.DataFim = dto.DataFim;
        if (dto.DataRescisao.HasValue) contrato.DataRescisao = dto.DataRescisao;
        if (dto.ValorMensal.HasValue) contrato.ValorMensal = dto.ValorMensal;
        if (dto.MultaRescisao.HasValue) contrato.MultaRescisao = dto.MultaRescisao;
        if (dto.Observacoes != null) contrato.Observacoes = dto.Observacoes;
        contrato.AtualizadoEm = DateTime.UtcNow;

        await _contratoRepo.AtualizarAsync(contrato);
        var full = await _contratoRepo.ObterComDetalhesAsync(id);
        return MapToDto(full!);
    }

    public async Task<ContratoDto?> RescindirAsync(Guid id, string? motivo)
    {
        var contrato = await _contratoRepo.ObterPorIdAsync(id);
        if (contrato == null) return null;

        contrato.Status = StatusContrato.Rescindido;
        contrato.DataRescisao = DateTime.UtcNow;
        contrato.Observacoes = string.IsNullOrEmpty(motivo) ? contrato.Observacoes : $"{contrato.Observacoes}\nRescisão: {motivo}";
        contrato.AtualizadoEm = DateTime.UtcNow;

        await _contratoRepo.AtualizarAsync(contrato);

        // Imóvel volta para análise
        var imovel = await _imovelRepo.ObterPorIdAsync(contrato.ImovelId);
        if (imovel != null) { imovel.Status = StatusImovel.EmAnalise; await _imovelRepo.AtualizarAsync(imovel); }

        var full = await _contratoRepo.ObterComDetalhesAsync(id);
        return MapToDto(full!);
    }

    public async Task<ContratoDto?> RenovarAsync(Guid id, DateTime novaDataFim, decimal? novoValor)
    {
        var contrato = await _contratoRepo.ObterPorIdAsync(id);
        if (contrato == null) return null;

        contrato.Status = StatusContrato.Renovado;
        contrato.AtualizadoEm = DateTime.UtcNow;
        await _contratoRepo.AtualizarAsync(contrato);

        // Cria novo contrato renovado
        var novo = new Contrato
        {
            Codigo = $"CTR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            ImovelId = contrato.ImovelId,
            ClienteId = contrato.ClienteId,
            CorretorId = contrato.CorretorId,
            Tipo = contrato.Tipo,
            DataInicio = contrato.DataFim ?? DateTime.UtcNow,
            DataFim = novaDataFim,
            ValorTotal = novoValor ?? contrato.ValorTotal,
            ValorMensal = novoValor ?? contrato.ValorMensal,
            MultaRescisao = contrato.MultaRescisao,
            Observacoes = $"Renovação do contrato {contrato.Codigo}"
        };

        await _contratoRepo.AdicionarAsync(novo);
        var full = await _contratoRepo.ObterComDetalhesAsync(novo.Id);
        return MapToDto(full!);
    }

    private static ContratoDto MapToDto(Contrato c) => new()
    {
        Id = c.Id, Codigo = c.Codigo,
        ImovelId = c.ImovelId, ImovelTitulo = c.Imovel?.Titulo ?? "", ImovelCodigo = c.Imovel?.Codigo ?? "",
        ClienteId = c.ClienteId, ClienteNome = c.Cliente?.Nome ?? "",
        CorretorId = c.CorretorId, CorretorNome = c.Corretor?.Nome,
        Tipo = c.Tipo.ToString(), TipoId = (int)c.Tipo,
        Status = c.Status.ToString(), StatusId = (int)c.Status,
        DataInicio = c.DataInicio, DataFim = c.DataFim, DataRescisao = c.DataRescisao,
        ValorTotal = c.ValorTotal, ValorMensal = c.ValorMensal, MultaRescisao = c.MultaRescisao,
        Observacoes = c.Observacoes, CriadoEm = c.CriadoEm
    };
}
