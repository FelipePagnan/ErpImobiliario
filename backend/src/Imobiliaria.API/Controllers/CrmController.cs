using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imobiliaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Gerente,Corretor")]
public class CrmController : ControllerBase
{
    private readonly ICrmService _service;
    public CrmController(ICrmService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> ObterTodos() => Ok(await _service.ObterTodosAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var i = await _service.ObterPorIdAsync(id);
        return i == null ? NotFound() : Ok(i);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] InteressadoCreateDto dto)
        => Ok(await _service.CriarAsync(dto));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] InteressadoUpdateDto dto)
    {
        var i = await _service.AtualizarAsync(id, dto);
        return i == null ? NotFound() : Ok(i);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
        => await _service.RemoverAsync(id) ? NoContent() : NotFound();

    [HttpPost("{id:guid}/contato")]
    public async Task<IActionResult> RegistrarContato(Guid id, [FromBody] ContatoRegistroDto dto)
    {
        var i = await _service.RegistrarContatoAsync(id, dto);
        return i == null ? NotFound() : Ok(i);
    }

    [HttpGet("{id:guid}/imoveis-compativeis")]
    public async Task<IActionResult> ImoveisCompativeis(Guid id)
        => Ok(await _service.ObterImoveisCompativeisAsync(id));
}
