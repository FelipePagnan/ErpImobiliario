using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Imobiliaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Realiza login e retorna o token JWT
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result == null)
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });
        return Ok(result);
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    [HttpPost("registrar")]
    public async Task<ActionResult<TokenDto>> Registrar([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegistrarAsync(dto);
        if (result == null)
            return BadRequest(new { mensagem = "E-mail já cadastrado." });
        return CreatedAtAction(nameof(Login), result);
    }
}
