using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Imobiliaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Gerente")]
public class FinanceiroController : ControllerBase
{
    private readonly IFinanceiroService _service;
    public FinanceiroController(IFinanceiroService service) => _service = service;

    [HttpGet("resumo")]
    public async Task<IActionResult> Resumo() => Ok(await _service.ObterResumoAsync());

    [HttpGet("lancamentos")]
    public async Task<IActionResult> Lancamentos() => Ok(await _service.ObterLancamentosAsync());

    [HttpGet("lancamentos/periodo")]
    public async Task<IActionResult> PorPeriodo([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        => Ok(await _service.ObterPorPeriodoAsync(inicio, fim));

    [HttpPost("lancamentos")]
    public async Task<IActionResult> CriarLancamento([FromBody] LancamentoCreateDto dto)
        => Ok(await _service.CriarLancamentoAsync(dto));

    [HttpPut("lancamentos/{id:guid}")]
    public async Task<IActionResult> AtualizarLancamento(Guid id, [FromBody] LancamentoUpdateDto dto)
    {
        var l = await _service.AtualizarLancamentoAsync(id, dto);
        return l == null ? NotFound() : Ok(l);
    }

    [HttpPost("lancamentos/{id:guid}/pagar")]
    public async Task<IActionResult> PagarLancamento(Guid id)
    {
        var l = await _service.PagarLancamentoAsync(id);
        return l == null ? NotFound() : Ok(l);
    }

    [HttpGet("comissoes")]
    public async Task<IActionResult> Comissoes() => Ok(await _service.ObterComissoesAsync());

    [HttpPost("comissoes")]
    public async Task<IActionResult> CriarComissao([FromBody] ComissaoCreateDto dto)
        => Ok(await _service.CriarComissaoAsync(dto));

    [HttpPost("comissoes/{id:guid}/pagar")]
    public async Task<IActionResult> PagarComissao(Guid id)
    {
        var c = await _service.PagarComissaoAsync(id);
        return c == null ? NotFound() : Ok(c);
    }
}
