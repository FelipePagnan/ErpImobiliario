using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Imobiliaria.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Imobiliaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteRepository _clienteRepo;

    public ClientesController(IClienteRepository clienteRepo)
    {
        _clienteRepo = clienteRepo;
    }

    /// <summary>
    /// Obtém o cliente vinculado ao usuário logado
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> ObterMeuPerfil()
    {
        var usuarioIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(usuarioIdStr)) return Unauthorized();

        var usuarioId = Guid.Parse(usuarioIdStr);
        var clientes = await _clienteRepo.BuscarAsync(c => c.UsuarioId == usuarioId && c.Ativo);
        var cliente = clientes.FirstOrDefault();

        if (cliente == null) return NotFound(new { mensagem = "Perfil de cliente não encontrado." });

        return Ok(new
        {
            cliente.Id,
            cliente.Nome,
            cliente.Email,
            cliente.Telefone,
            cliente.UsuarioId
        });
    }
}
