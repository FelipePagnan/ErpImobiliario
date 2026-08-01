using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Enums;
using Imobiliaria.Domain.Interfaces;

namespace Imobiliaria.Application.Services;

public class VisitaService : IVisitaService
{
    private readonly IVisitaRepository _visitaRepo;
    private readonly IImovelRepository _imovelRepo;

    public VisitaService(IVisitaRepository visitaRepo, IImovelRepository imovelRepo)
    {
        _visitaRepo = visitaRepo;
        _imovelRepo = imovelRepo;
    }

    public async Task<IEnumerable<VisitaDto>> ObterTodasAsync()
    {
        var visitas = await _visitaRepo.ObterComDetalhesAsync();
        return visitas.Select(MapToDto);
    }

    public async Task<VisitaDto?> ObterPorIdAsync(Guid id)
    {
        var v = await _visitaRepo.ObterComDetalhesAsync(id);
        return v == null ? null : MapToDto(v);
    }

    public async Task<IEnumerable<VisitaDto>> ObterPorClienteAsync(Guid clienteId)
    {
        var visitas = await _visitaRepo.ObterPorClienteAsync(clienteId);
        return visitas.Select(MapToDto);
    }

    public async Task<IEnumerable<VisitaDto>> ObterPorCorretorAsync(Guid corretorId)
    {
        var visitas = await _visitaRepo.ObterPorCorretorAsync(corretorId);
        return visitas.Select(MapToDto);
    }

    public async Task<VisitaDto> CriarAsync(VisitaCreateDto dto)
    {
        var imovel = await _imovelRepo.ObterComDetalhesPorIdAsync(dto.ImovelId);
        var visita = new Visita
        {
            ImovelId = dto.ImovelId,
            ClienteId = dto.ClienteId,
            CorretorId = imovel?.CorretorId,
            DataAgendada = dto.DataAgendada,
            Observacoes = dto.Observacoes,
            Status = dto.DataAgendada.HasValue ? StatusVisita.Agendada : StatusVisita.Solicitada
        };

        await _visitaRepo.AdicionarAsync(visita);
        var full = await _visitaRepo.ObterComDetalhesAsync(visita.Id);
        return MapToDto(full!);
    }

    public async Task<VisitaDto?> AtualizarAsync(Guid id, VisitaUpdateDto dto)
    {
        var visita = await _visitaRepo.ObterPorIdAsync(id);
        if (visita == null) return null;

        if (dto.DataAgendada.HasValue) visita.DataAgendada = dto.DataAgendada;
        if (dto.Status.HasValue) visita.Status = dto.Status.Value;
        if (dto.CorretorId.HasValue) visita.CorretorId = dto.CorretorId;
        if (dto.Observacoes != null) visita.Observacoes = dto.Observacoes;
        if (dto.FeedbackCliente != null) visita.FeedbackCliente = dto.FeedbackCliente;
        visita.AtualizadoEm = DateTime.UtcNow;

        await _visitaRepo.AtualizarAsync(visita);
        var full = await _visitaRepo.ObterComDetalhesAsync(id);
        return MapToDto(full!);
    }

    public async Task<bool> CancelarAsync(Guid id)
    {
        var visita = await _visitaRepo.ObterPorIdAsync(id);
        if (visita == null) return false;
        visita.Status = StatusVisita.Cancelada;
        visita.AtualizadoEm = DateTime.UtcNow;
        await _visitaRepo.AtualizarAsync(visita);
        return true;
    }

    private static VisitaDto MapToDto(Visita v) => new()
    {
        Id = v.Id,
        ImovelId = v.ImovelId, ImovelTitulo = v.Imovel?.Titulo ?? "", ImovelCodigo = v.Imovel?.Codigo ?? "",
        ClienteId = v.ClienteId, ClienteNome = v.Cliente?.Nome ?? "", ClienteTelefone = v.Cliente?.Telefone,
        CorretorId = v.CorretorId, CorretorNome = v.Corretor?.Nome,
        DataSolicitacao = v.DataSolicitacao,
        DataAgendada = v.DataAgendada,
        Status = v.Status.ToString(), StatusId = (int)v.Status,
        Observacoes = v.Observacoes,
        FeedbackCliente = v.FeedbackCliente
    };
}
