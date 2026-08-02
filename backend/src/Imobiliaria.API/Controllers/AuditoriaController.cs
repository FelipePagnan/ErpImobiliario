using Imobiliaria.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imobiliaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class AuditoriaController : ControllerBase
{
    private readonly IAuditoriaService _service;
    public AuditoriaController(IAuditoriaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> ObterTodos([FromQuery] int limite = 100)
        => Ok(await _service.ObterTodosAsync(limite));

    [HttpGet("{entidade}/{entidadeId:guid}")]
    public async Task<IActionResult> PorEntidade(string entidade, Guid entidadeId)
        => Ok(await _service.ObterPorEntidadeAsync(entidade, entidadeId));
}
