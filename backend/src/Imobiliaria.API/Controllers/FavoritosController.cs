using Imobiliaria.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imobiliaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritosController : ControllerBase
{
    private readonly IFavoritoService _service;
    public FavoritosController(IFavoritoService service) => _service = service;

    [HttpGet("{clienteId:guid}")]
    public async Task<IActionResult> ObterFavoritos(Guid clienteId)
        => Ok(await _service.ObterFavoritosAsync(clienteId));

    [HttpPost("{clienteId:guid}/{imovelId:guid}")]
    public async Task<IActionResult> Adicionar(Guid clienteId, Guid imovelId)
    {
        await _service.AdicionarFavoritoAsync(clienteId, imovelId);
        return Ok(new { mensagem = "Favorito adicionado." });
    }

    [HttpDelete("{clienteId:guid}/{imovelId:guid}")]
    public async Task<IActionResult> Remover(Guid clienteId, Guid imovelId)
    {
        var ok = await _service.RemoverFavoritoAsync(clienteId, imovelId);
        return ok ? Ok(new { mensagem = "Favorito removido." }) : NotFound();
    }

    [HttpGet("{clienteId:guid}/{imovelId:guid}/check")]
    public async Task<IActionResult> Verificar(Guid clienteId, Guid imovelId)
        => Ok(new { favorito = await _service.EhFavoritoAsync(clienteId, imovelId) });
}
