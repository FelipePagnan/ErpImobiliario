using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imobiliaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImoveisController : ControllerBase
{
    private readonly IImovelService _service;

    public ImoveisController(IImovelService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista todos os imóveis ativos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ImovelDto>>> ObterTodos()
    {
        var imoveis = await _service.ObterTodosAsync();
        return Ok(imoveis);
    }

    /// <summary>
    /// Obtém um imóvel por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ImovelDto>> ObterPorId(Guid id)
    {
        var imovel = await _service.ObterPorIdAsync(id);
        if (imovel == null) return NotFound();
        return Ok(imovel);
    }

    /// <summary>
    /// Filtra imóveis por critérios
    /// </summary>
    [HttpGet("filtrar")]
    public async Task<ActionResult<IEnumerable<ImovelDto>>> Filtrar([FromQuery] ImovelFilterDto filtro)
    {
        var imoveis = await _service.FiltrarAsync(filtro);
        return Ok(imoveis);
    }

    /// <summary>
    /// Cria um novo imóvel
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador,Gerente")]
    public async Task<ActionResult<ImovelDto>> Criar([FromBody] ImovelCreateDto dto)
    {
        var imovel = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = imovel.Id }, imovel);
    }

    /// <summary>
    /// Atualiza um imóvel
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador,Gerente,Corretor")]
    public async Task<ActionResult<ImovelDto>> Atualizar(Guid id, [FromBody] ImovelUpdateDto dto)
    {
        var imovel = await _service.AtualizarAsync(id, dto);
        if (imovel == null) return NotFound();
        return Ok(imovel);
    }

    /// <summary>
    /// Remove (desativa) um imóvel
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> Remover(Guid id)
    {
        var ok = await _service.RemoverAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Dados do dashboard
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize]
    public async Task<ActionResult<DashboardDto>> Dashboard()
    {
        var data = await _service.ObterDashboardAsync();
        return Ok(data);
    }
}
