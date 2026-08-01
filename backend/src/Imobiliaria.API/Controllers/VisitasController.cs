using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imobiliaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitasController : ControllerBase
{
    private readonly IVisitaService _service;
    public VisitasController(IVisitaService service) => _service = service;

    [HttpGet]
    [Authorize(Roles = "Administrador,Gerente,Corretor")]
    public async Task<IActionResult> ObterTodas() => Ok(await _service.ObterTodasAsync());

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var v = await _service.ObterPorIdAsync(id);
        return v == null ? NotFound() : Ok(v);
    }

    [HttpGet("cliente/{clienteId:guid}")]
    [Authorize]
    public async Task<IActionResult> PorCliente(Guid clienteId) => Ok(await _service.ObterPorClienteAsync(clienteId));

    [HttpGet("corretor/{corretorId:guid}")]
    [Authorize(Roles = "Administrador,Gerente,Corretor")]
    public async Task<IActionResult> PorCorretor(Guid corretorId) => Ok(await _service.ObterPorCorretorAsync(corretorId));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Criar([FromBody] VisitaCreateDto dto)
    {
        var v = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = v.Id }, v);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador,Gerente,Corretor")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] VisitaUpdateDto dto)
    {
        var v = await _service.AtualizarAsync(id, dto);
        return v == null ? NotFound() : Ok(v);
    }

    [HttpPost("{id:guid}/cancelar")]
    [Authorize]
    public async Task<IActionResult> Cancelar(Guid id)
        => await _service.CancelarAsync(id) ? Ok(new { mensagem = "Visita cancelada." }) : NotFound();
}
