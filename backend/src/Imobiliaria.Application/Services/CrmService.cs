using System.Text.Json;
using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Enums;
using Imobiliaria.Domain.Interfaces;

namespace Imobiliaria.Application.Services;

public class CrmService : ICrmService
{
    private readonly IInteressadoRepository _interessadoRepo;
    private readonly IImovelRepository _imovelRepo;

    public CrmService(IInteressadoRepository interessadoRepo, IImovelRepository imovelRepo)
    {
        _interessadoRepo = interessadoRepo;
        _imovelRepo = imovelRepo;
    }

    public async Task<IEnumerable<InteressadoDto>> ObterTodosAsync()
    {
        var interessados = await _interessadoRepo.ObterComDetalhesAsync();
        var dtos = new List<InteressadoDto>();
        foreach (var i in interessados.Where(x => x.Ativo))
        {
            var dto = MapToDto(i);
            dto.ImoveisCompativeis = (await ContarCompativeisAsync(i));
            dtos.Add(dto);
        }
        return dtos;
    }

    public async Task<InteressadoDto?> ObterPorIdAsync(Guid id)
    {
        var i = await _interessadoRepo.ObterComDetalhesAsync(id);
        if (i == null) return null;
        var dto = MapToDto(i);
        dto.ImoveisCompativeis = await ContarCompativeisAsync(i);
        return dto;
    }

    public async Task<InteressadoDto> CriarAsync(InteressadoCreateDto dto)
    {
        var interessado = new Interessado
        {
            Nome = dto.Nome, Email = dto.Email, Telefone = dto.Telefone,
            ClienteId = dto.ClienteId,
            CidadeDesejada = dto.CidadeDesejada, BairroDesejado = dto.BairroDesejado,
            TipoImovelDesejado = dto.TipoImovelDesejado, FinalidadeDesejada = dto.FinalidadeDesejada,
            OrcamentoMinimo = dto.OrcamentoMinimo, OrcamentoMaximo = dto.OrcamentoMaximo,
            DormitoriosMinimo = dto.DormitoriosMinimo, AreaMinima = dto.AreaMinima,
            Observacoes = dto.Observacoes
        };
        await _interessadoRepo.AdicionarAsync(interessado);
        return MapToDto(interessado);
    }

    public async Task<InteressadoDto?> AtualizarAsync(Guid id, InteressadoUpdateDto dto)
    {
        var i = await _interessadoRepo.ObterPorIdAsync(id);
        if (i == null) return null;

        if (dto.Nome != null) i.Nome = dto.Nome;
        if (dto.Email != null) i.Email = dto.Email;
        if (dto.Telefone != null) i.Telefone = dto.Telefone;
        if (dto.CidadeDesejada != null) i.CidadeDesejada = dto.CidadeDesejada;
        if (dto.BairroDesejado != null) i.BairroDesejado = dto.BairroDesejado;
        if (dto.TipoImovelDesejado.HasValue) i.TipoImovelDesejado = dto.TipoImovelDesejado;
        if (dto.FinalidadeDesejada.HasValue) i.FinalidadeDesejada = dto.FinalidadeDesejada;
        if (dto.OrcamentoMinimo.HasValue) i.OrcamentoMinimo = dto.OrcamentoMinimo;
        if (dto.OrcamentoMaximo.HasValue) i.OrcamentoMaximo = dto.OrcamentoMaximo;
        if (dto.DormitoriosMinimo.HasValue) i.DormitoriosMinimo = dto.DormitoriosMinimo;
        if (dto.AreaMinima.HasValue) i.AreaMinima = dto.AreaMinima;
        if (dto.Observacoes != null) i.Observacoes = dto.Observacoes;
        if (dto.Notificar.HasValue) i.Notificar = dto.Notificar.Value;
        i.AtualizadoEm = DateTime.UtcNow;

        await _interessadoRepo.AtualizarAsync(i);
        return MapToDto(i);
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        var i = await _interessadoRepo.ObterPorIdAsync(id);
        if (i == null) return false;
        i.Ativo = false;
        await _interessadoRepo.AtualizarAsync(i);
        return true;
    }

    public async Task<InteressadoDto?> RegistrarContatoAsync(Guid id, ContatoRegistroDto dto)
    {
        var i = await _interessadoRepo.ObterPorIdAsync(id);
        if (i == null) return null;

        var historico = new List<ContatoHistoricoDto>();
        if (!string.IsNullOrEmpty(i.HistoricoContatosJson))
            historico = JsonSerializer.Deserialize<List<ContatoHistoricoDto>>(i.HistoricoContatosJson) ?? new();

        historico.Add(new ContatoHistoricoDto { Data = DateTime.UtcNow, Tipo = dto.Tipo, Descricao = dto.Descricao });
        i.HistoricoContatosJson = JsonSerializer.Serialize(historico);
        i.UltimoContato = DateTime.UtcNow;
        i.AtualizadoEm = DateTime.UtcNow;

        await _interessadoRepo.AtualizarAsync(i);
        return MapToDto(i);
    }

    public async Task<IEnumerable<ImovelDto>> ObterImoveisCompativeisAsync(Guid interessadoId)
    {
        var i = await _interessadoRepo.ObterPorIdAsync(interessadoId);
        if (i == null) return Enumerable.Empty<ImovelDto>();

        var imoveis = await _imovelRepo.FiltrarAsync(
            i.CidadeDesejada, i.BairroDesejado, i.TipoImovelDesejado, i.FinalidadeDesejada,
            i.OrcamentoMinimo, i.OrcamentoMaximo, i.DormitoriosMinimo, i.AreaMinima, null);

        return imoveis.Select(im => new ImovelDto
        {
            Id = im.Id, Titulo = im.Titulo, Codigo = im.Codigo,
            Tipo = im.Tipo.ToString(), Finalidade = im.Finalidade.ToString(),
            PrecoVenda = im.PrecoVenda, PrecoLocacao = im.PrecoLocacao,
            AreaTotal = im.AreaTotal, Dormitorios = im.Dormitorios,
            FotoPrincipalUrl = im.FotoPrincipalUrl,
            Endereco = im.Endereco != null ? new EnderecoDto
            { Bairro = im.Endereco.Bairro, Cidade = im.Endereco.Cidade, Estado = im.Endereco.Estado } : null
        });
    }

    private async Task<int> ContarCompativeisAsync(Interessado i)
    {
        var imoveis = await _imovelRepo.FiltrarAsync(
            i.CidadeDesejada, i.BairroDesejado, i.TipoImovelDesejado, i.FinalidadeDesejada,
            i.OrcamentoMinimo, i.OrcamentoMaximo, i.DormitoriosMinimo, i.AreaMinima, null);
        return imoveis.Count();
    }

    private static InteressadoDto MapToDto(Interessado i)
    {
        List<ContatoHistoricoDto>? historico = null;
        if (!string.IsNullOrEmpty(i.HistoricoContatosJson))
            historico = JsonSerializer.Deserialize<List<ContatoHistoricoDto>>(i.HistoricoContatosJson);

        return new InteressadoDto
        {
            Id = i.Id, Nome = i.Nome, Email = i.Email, Telefone = i.Telefone,
            ClienteId = i.ClienteId, ClienteNome = i.Cliente?.Nome,
            CidadeDesejada = i.CidadeDesejada, BairroDesejado = i.BairroDesejado,
            TipoImovelDesejado = i.TipoImovelDesejado,
            TipoImovelNome = i.TipoImovelDesejado.HasValue ? ((TipoImovel)i.TipoImovelDesejado).ToString() : null,
            FinalidadeDesejada = i.FinalidadeDesejada,
            FinalidadeNome = i.FinalidadeDesejada.HasValue ? ((FinalidadeImovel)i.FinalidadeDesejada).ToString() : null,
            OrcamentoMinimo = i.OrcamentoMinimo, OrcamentoMaximo = i.OrcamentoMaximo,
            DormitoriosMinimo = i.DormitoriosMinimo, AreaMinima = i.AreaMinima,
            Observacoes = i.Observacoes, Notificar = i.Notificar,
            UltimoContato = i.UltimoContato,
            HistoricoContatos = historico,
            CriadoEm = i.CriadoEm
        };
    }
}
