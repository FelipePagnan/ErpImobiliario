using System.Text.Json;
using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Interfaces;

namespace Imobiliaria.Application.Services;

public class FavoritoService : IFavoritoService
{
    private readonly IFavoritoRepository _favoritoRepo;
    private readonly IImovelRepository _imovelRepo;

    public FavoritoService(IFavoritoRepository favoritoRepo, IImovelRepository imovelRepo)
    {
        _favoritoRepo = favoritoRepo;
        _imovelRepo = imovelRepo;
    }

    public async Task<IEnumerable<ImovelDto>> ObterFavoritosAsync(Guid clienteId)
    {
        var favoritos = await _favoritoRepo.ObterPorClienteAsync(clienteId);
        return favoritos.Select(f => MapImovelToDto(f.Imovel));
    }

    public async Task<bool> AdicionarFavoritoAsync(Guid clienteId, Guid imovelId)
    {
        var existente = await _favoritoRepo.ObterPorClienteEImovelAsync(clienteId, imovelId);
        if (existente != null)
        {
            if (!existente.Ativo) { existente.Ativo = true; await _favoritoRepo.AtualizarAsync(existente); }
            return true;
        }

        await _favoritoRepo.AdicionarAsync(new Favorito { ClienteId = clienteId, ImovelId = imovelId });
        return true;
    }

    public async Task<bool> RemoverFavoritoAsync(Guid clienteId, Guid imovelId)
    {
        var fav = await _favoritoRepo.ObterPorClienteEImovelAsync(clienteId, imovelId);
        if (fav == null) return false;
        fav.Ativo = false;
        await _favoritoRepo.AtualizarAsync(fav);
        return true;
    }

    public async Task<bool> EhFavoritoAsync(Guid clienteId, Guid imovelId)
    {
        var fav = await _favoritoRepo.ObterPorClienteEImovelAsync(clienteId, imovelId);
        return fav != null && fav.Ativo;
    }

    private static ImovelDto MapImovelToDto(Imovel i) => new()
    {
        Id = i.Id, Titulo = i.Titulo, Codigo = i.Codigo,
        Tipo = i.Tipo.ToString(), TipoId = (int)i.Tipo,
        Finalidade = i.Finalidade.ToString(), FinalidadeId = (int)i.Finalidade,
        Status = i.Status.ToString(), StatusId = (int)i.Status,
        PrecoVenda = i.PrecoVenda, PrecoLocacao = i.PrecoLocacao,
        AreaTotal = i.AreaTotal, Dormitorios = i.Dormitorios,
        Banheiros = i.Banheiros, VagasGaragem = i.VagasGaragem,
        FotoPrincipalUrl = i.FotoPrincipalUrl,
        Endereco = i.Endereco != null ? new EnderecoDto
        {
            Logradouro = i.Endereco.Logradouro, Numero = i.Endereco.Numero,
            Bairro = i.Endereco.Bairro, Cidade = i.Endereco.Cidade, Estado = i.Endereco.Estado
        } : null,
        CriadoEm = i.CriadoEm
    };
}
