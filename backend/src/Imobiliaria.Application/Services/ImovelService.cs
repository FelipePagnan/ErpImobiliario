using System.Text.Json;
using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Enums;
using Imobiliaria.Domain.Interfaces;

namespace Imobiliaria.Application.Services;

public class ImovelService : IImovelService
{
    private readonly IImovelRepository _imovelRepo;
    private readonly IClienteRepository _clienteRepo;
    private readonly ICorretorRepository _corretorRepo;
    private readonly IProprietarioRepository _proprietarioRepo;

    public ImovelService(
        IImovelRepository imovelRepo,
        IClienteRepository clienteRepo,
        ICorretorRepository corretorRepo,
        IProprietarioRepository proprietarioRepo)
    {
        _imovelRepo = imovelRepo;
        _clienteRepo = clienteRepo;
        _corretorRepo = corretorRepo;
        _proprietarioRepo = proprietarioRepo;
    }

    public async Task<IEnumerable<ImovelDto>> ObterTodosAsync()
    {
        var imoveis = await _imovelRepo.ObterComDetalhesAsync();
        return imoveis.Select(MapToDto);
    }

    public async Task<ImovelDto?> ObterPorIdAsync(Guid id)
    {
        var imovel = await _imovelRepo.ObterComDetalhesPorIdAsync(id);
        return imovel == null ? null : MapToDto(imovel);
    }

    public async Task<IEnumerable<ImovelDto>> FiltrarAsync(ImovelFilterDto filtro)
    {
        var imoveis = await _imovelRepo.FiltrarAsync(
            filtro.Cidade, filtro.Bairro, filtro.Tipo, filtro.Finalidade,
            filtro.PrecoMin, filtro.PrecoMax, filtro.DormitoriosMin,
            filtro.AreaMin, filtro.VagasMin);
        return imoveis.Select(MapToDto);
    }

    public async Task<ImovelDto> CriarAsync(ImovelCreateDto dto)
    {
        var endereco = new Endereco
        {
            Logradouro = dto.Endereco.Logradouro,
            Numero = dto.Endereco.Numero,
            Complemento = dto.Endereco.Complemento,
            Bairro = dto.Endereco.Bairro,
            Cidade = dto.Endereco.Cidade,
            Estado = dto.Endereco.Estado,
            CEP = dto.Endereco.CEP
        };

        var imovel = new Imovel
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Codigo = GerarCodigo(dto.Tipo),
            Tipo = dto.Tipo,
            Finalidade = dto.Finalidade,
            PrecoVenda = dto.PrecoVenda,
            PrecoLocacao = dto.PrecoLocacao,
            ValorCondominio = dto.ValorCondominio,
            ValorIPTU = dto.ValorIPTU,
            AreaTotal = dto.AreaTotal,
            AreaConstruida = dto.AreaConstruida,
            Dormitorios = dto.Dormitorios,
            Suites = dto.Suites,
            Banheiros = dto.Banheiros,
            VagasGaragem = dto.VagasGaragem,
            Andares = dto.Andares,
            Andar = dto.Andar,
            Mobiliado = dto.Mobiliado,
            FotoPrincipalUrl = dto.FotoPrincipalUrl,
            Endereco = endereco,
            EnderecoId = endereco.Id,
            ProprietarioId = dto.ProprietarioId,
            CorretorId = dto.CorretorId,
            CaracteristicasJson = dto.Caracteristicas != null
                ? JsonSerializer.Serialize(dto.Caracteristicas)
                : null
        };

        var created = await _imovelRepo.AdicionarAsync(imovel);
        var full = await _imovelRepo.ObterComDetalhesPorIdAsync(created.Id);
        return MapToDto(full!);
    }

    public async Task<ImovelDto?> AtualizarAsync(Guid id, ImovelUpdateDto dto)
    {
        var imovel = await _imovelRepo.ObterPorIdAsync(id);
        if (imovel == null) return null;

        if (dto.Titulo != null) imovel.Titulo = dto.Titulo;
        if (dto.Descricao != null) imovel.Descricao = dto.Descricao;
        if (dto.Tipo.HasValue) imovel.Tipo = dto.Tipo.Value;
        if (dto.Finalidade.HasValue) imovel.Finalidade = dto.Finalidade.Value;
        if (dto.Status.HasValue) imovel.Status = dto.Status.Value;
        if (dto.PrecoVenda.HasValue) imovel.PrecoVenda = dto.PrecoVenda;
        if (dto.PrecoLocacao.HasValue) imovel.PrecoLocacao = dto.PrecoLocacao;
        if (dto.ValorCondominio.HasValue) imovel.ValorCondominio = dto.ValorCondominio;
        if (dto.ValorIPTU.HasValue) imovel.ValorIPTU = dto.ValorIPTU;
        if (dto.AreaTotal.HasValue) imovel.AreaTotal = dto.AreaTotal.Value;
        if (dto.AreaConstruida.HasValue) imovel.AreaConstruida = dto.AreaConstruida;
        if (dto.Dormitorios.HasValue) imovel.Dormitorios = dto.Dormitorios.Value;
        if (dto.Suites.HasValue) imovel.Suites = dto.Suites;
        if (dto.Banheiros.HasValue) imovel.Banheiros = dto.Banheiros.Value;
        if (dto.VagasGaragem.HasValue) imovel.VagasGaragem = dto.VagasGaragem.Value;
        if (dto.Andares.HasValue) imovel.Andares = dto.Andares;
        if (dto.Andar.HasValue) imovel.Andar = dto.Andar;
        if (dto.Mobiliado.HasValue) imovel.Mobiliado = dto.Mobiliado;
        if (dto.FotoPrincipalUrl != null) imovel.FotoPrincipalUrl = dto.FotoPrincipalUrl;
        if (dto.CorretorId.HasValue) imovel.CorretorId = dto.CorretorId;
        if (dto.Caracteristicas != null)
            imovel.CaracteristicasJson = JsonSerializer.Serialize(dto.Caracteristicas);

        imovel.AtualizadoEm = DateTime.UtcNow;
        await _imovelRepo.AtualizarAsync(imovel);

        var full = await _imovelRepo.ObterComDetalhesPorIdAsync(id);
        return MapToDto(full!);
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        var imovel = await _imovelRepo.ObterPorIdAsync(id);
        if (imovel == null) return false;

        imovel.Ativo = false;
        imovel.AtualizadoEm = DateTime.UtcNow;
        await _imovelRepo.AtualizarAsync(imovel);
        return true;
    }

    public async Task<DashboardDto> ObterDashboardAsync()
    {
        var disponiveis = await _imovelRepo.ContarAsync(i => i.Status == StatusImovel.Disponivel && i.Ativo);
        var alugados = await _imovelRepo.ContarAsync(i => i.Status == StatusImovel.Alugado && i.Ativo);
        var vendidos = await _imovelRepo.ContarAsync(i => i.Status == StatusImovel.Vendido && i.Ativo);
        var clientes = await _clienteRepo.ContarAsync(c => c.Ativo);
        var corretores = await _corretorRepo.ContarAsync(c => c.Ativo);
        var proprietarios = await _proprietarioRepo.ContarAsync(p => p.Ativo);

        var recentes = (await _imovelRepo.ObterComDetalhesAsync())
            .OrderByDescending(i => i.CriadoEm)
            .Take(5)
            .Select(MapToDto)
            .ToList();

        return new DashboardDto
        {
            ImoveisDisponiveis = disponiveis,
            ImoveisAlugados = alugados,
            ImoveisVendidos = vendidos,
            TotalClientes = clientes,
            TotalCorretores = corretores,
            TotalProprietarios = proprietarios,
            ImoveisRecentes = recentes
        };
    }

    // --- Helpers ---

    private static ImovelDto MapToDto(Imovel i) => new()
    {
        Id = i.Id,
        Titulo = i.Titulo,
        Descricao = i.Descricao,
        Codigo = i.Codigo,
        Tipo = i.Tipo.ToString(),
        TipoId = (int)i.Tipo,
        Finalidade = i.Finalidade.ToString(),
        FinalidadeId = (int)i.Finalidade,
        Status = i.Status.ToString(),
        StatusId = (int)i.Status,
        PrecoVenda = i.PrecoVenda,
        PrecoLocacao = i.PrecoLocacao,
        ValorCondominio = i.ValorCondominio,
        ValorIPTU = i.ValorIPTU,
        AreaTotal = i.AreaTotal,
        AreaConstruida = i.AreaConstruida,
        Dormitorios = i.Dormitorios,
        Suites = i.Suites,
        Banheiros = i.Banheiros,
        VagasGaragem = i.VagasGaragem,
        Andares = i.Andares,
        Andar = i.Andar,
        Mobiliado = i.Mobiliado,
        FotoPrincipalUrl = i.FotoPrincipalUrl,
        Fotos = DeserializeList(i.FotosJson),
        Caracteristicas = DeserializeList(i.CaracteristicasJson),
        Endereco = i.Endereco != null ? new EnderecoDto
        {
            Logradouro = i.Endereco.Logradouro,
            Numero = i.Endereco.Numero,
            Complemento = i.Endereco.Complemento,
            Bairro = i.Endereco.Bairro,
            Cidade = i.Endereco.Cidade,
            Estado = i.Endereco.Estado,
            CEP = i.Endereco.CEP
        } : null,
        ProprietarioNome = i.Proprietario?.Nome,
        CorretorNome = i.Corretor?.Nome,
        CorretorTelefone = i.Corretor?.Telefone,
        CriadoEm = i.CriadoEm
    };

    private static List<string>? DeserializeList(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try { return JsonSerializer.Deserialize<List<string>>(json); }
        catch { return null; }
    }

    private static string GerarCodigo(TipoImovel tipo)
    {
        var prefixo = tipo switch
        {
            TipoImovel.Casa => "CAS",
            TipoImovel.Apartamento => "APT",
            TipoImovel.Cobertura => "COB",
            TipoImovel.Studio => "STD",
            TipoImovel.Kitnet => "KIT",
            TipoImovel.Sobrado => "SOB",
            TipoImovel.Terreno => "TER",
            TipoImovel.Chacara => "CHA",
            TipoImovel.Fazenda => "FAZ",
            TipoImovel.Galpao => "GAL",
            TipoImovel.SalaComercial => "SAL",
            TipoImovel.Loja => "LOJ",
            TipoImovel.AreaIndustrial => "IND",
            _ => "IMO"
        };
        return $"{prefixo}-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
    }
}
