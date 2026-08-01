using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Interfaces;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Imobiliaria.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IConfiguration _config;

    public AuthService(IUsuarioRepository usuarioRepo, IConfiguration config)
    {
        _usuarioRepo = usuarioRepo;
        _config = config;
    }

    public async Task<TokenDto?> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarioRepo.ObterPorEmailAsync(dto.Email);
        if (usuario == null || !VerificarSenha(dto.Senha, usuario.SenhaHash))
            return null;

        usuario.UltimoLogin = DateTime.UtcNow;
        await _usuarioRepo.AtualizarAsync(usuario);

        return GerarToken(usuario);
    }

    public async Task<TokenDto?> RegistrarAsync(RegisterDto dto)
    {
        var existente = await _usuarioRepo.ObterPorEmailAsync(dto.Email);
        if (existente != null) return null;

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = HashSenha(dto.Senha),
            Perfil = dto.Perfil,
            Telefone = dto.Telefone
        };

        await _usuarioRepo.AdicionarAsync(usuario);
        return GerarToken(usuario);
    }

    private TokenDto GerarToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Secret"] ?? "ChaveSecretaPadrao12345678901234567890"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiracao = DateTime.UtcNow.AddHours(8);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "ImobiliariaERP",
            audience: _config["Jwt:Audience"] ?? "ImobiliariaERP",
            claims: claims,
            expires: expiracao,
            signingCredentials: creds);

        return new TokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiracao = expiracao,
            Usuario = new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil.ToString(),
                PerfilId = (int)usuario.Perfil
            }
        };
    }

    private static string HashSenha(string senha)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerificarSenha(string senha, string hash)
    {
        return HashSenha(senha) == hash;
    }
}
