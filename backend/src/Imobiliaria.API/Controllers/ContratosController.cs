using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imobiliaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Gerente")]
public class ContratosController : ControllerBase
{
    private readonly IContratoService _service;
    public ContratosController(IContratoService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> ObterTodos() => Ok(await _service.ObterTodosAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var c = await _service.ObterPorIdAsync(id);
        return c == null ? NotFound() : Ok(c);
    }

    [HttpGet("vencendo")]
    public async Task<IActionResult> Vencendo([FromQuery] int dias = 30)
        => Ok(await _service.ObterVencendoAsync(dias));

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ContratoCreateDto dto)
    {
        var c = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = c.Id }, c);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] ContratoUpdateDto dto)
    {
        var c = await _service.AtualizarAsync(id, dto);
        return c == null ? NotFound() : Ok(c);
    }

    [HttpPost("{id:guid}/rescindir")]
    public async Task<IActionResult> Rescindir(Guid id, [FromBody] RescisaoDto? dto)
    {
        var c = await _service.RescindirAsync(id, dto?.Motivo);
        return c == null ? NotFound() : Ok(c);
    }

    [HttpPost("{id:guid}/renovar")]
    public async Task<IActionResult> Renovar(Guid id, [FromBody] RenovacaoDto dto)
    {
        var c = await _service.RenovarAsync(id, dto.NovaDataFim, dto.NovoValor);
        return c == null ? NotFound() : Ok(c);
    }
}

public class RescisaoDto { public string? Motivo { get; set; } }
public class RenovacaoDto { public DateTime NovaDataFim { get; set; } public decimal? NovoValor { get; set; } }
