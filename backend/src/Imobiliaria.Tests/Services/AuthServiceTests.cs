using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Imobiliaria.Application.DTOs;
using Imobiliaria.Application.Services;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Enums;
using Imobiliaria.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Imobiliaria.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _usuarioRepoMock = new Mock<IUsuarioRepository>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "ChaveSecretaParaTestesUnitarios12345678",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience"
            })
            .Build();

        _service = new AuthService(_usuarioRepoMock.Object, config);
    }

    private static string HashSenha(string senha)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(senha)));
    }

    private Usuario CriarUsuarioFake(string email = "teste@email.com", string senha = "123456",
        PerfilUsuario perfil = PerfilUsuario.Cliente)
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Usuário Teste",
            Email = email,
            SenhaHash = HashSenha(senha),
            Perfil = perfil
        };
    }

    // ===== LoginAsync =====

    [Fact]
    public async Task LoginAsync_CredenciaisValidas_DeveRetornarToken()
    {
        var usuario = CriarUsuarioFake();
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync("teste@email.com")).ReturnsAsync(usuario);

        var resultado = await _service.LoginAsync(new LoginDto { Email = "teste@email.com", Senha = "123456" });

        resultado.Should().NotBeNull();
        resultado!.Token.Should().NotBeNullOrEmpty();
        resultado.Usuario.Should().NotBeNull();
        resultado.Usuario.Email.Should().Be("teste@email.com");
        resultado.Expiracao.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_SenhaInvalida_DeveRetornarNull()
    {
        var usuario = CriarUsuarioFake();
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync("teste@email.com")).ReturnsAsync(usuario);

        var resultado = await _service.LoginAsync(new LoginDto { Email = "teste@email.com", Senha = "senhaerrada" });

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_EmailInexistente_DeveRetornarNull()
    {
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync(It.IsAny<string>())).ReturnsAsync((Usuario?)null);

        var resultado = await _service.LoginAsync(new LoginDto { Email = "naoexiste@email.com", Senha = "123" });

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_DeveAtualizarUltimoLogin()
    {
        var usuario = CriarUsuarioFake();
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync("teste@email.com")).ReturnsAsync(usuario);

        await _service.LoginAsync(new LoginDto { Email = "teste@email.com", Senha = "123456" });

        usuario.UltimoLogin.Should().NotBeNull();
        _usuarioRepoMock.Verify(r => r.AtualizarAsync(usuario), Times.Once);
    }

    // ===== RegistrarAsync =====

    [Fact]
    public async Task RegistrarAsync_EmailNovo_DeveCriarERetornarToken()
    {
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync(It.IsAny<string>())).ReturnsAsync((Usuario?)null);
        _usuarioRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Usuario>())).ReturnsAsync((Usuario u) => u);

        var dto = new RegisterDto { Nome = "Novo Usuário", Email = "novo@email.com", Senha = "123456", Perfil = PerfilUsuario.Cliente };

        var resultado = await _service.RegistrarAsync(dto);

        resultado.Should().NotBeNull();
        resultado!.Token.Should().NotBeNullOrEmpty();
        resultado.Usuario.Nome.Should().Be("Novo Usuário");
        resultado.Usuario.Perfil.Should().Be("Cliente");
        _usuarioRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_EmailJaExistente_DeveRetornarNull()
    {
        var existente = CriarUsuarioFake("existente@email.com");
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync("existente@email.com")).ReturnsAsync(existente);

        var dto = new RegisterDto { Nome = "Outro", Email = "existente@email.com", Senha = "123456" };

        var resultado = await _service.RegistrarAsync(dto);

        resultado.Should().BeNull();
        _usuarioRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Never);
    }

    // ===== Token =====

    [Fact]
    public async Task LoginAsync_TokenDeveConterPerfilCorreto()
    {
        var admin = CriarUsuarioFake(perfil: PerfilUsuario.Administrador);
        _usuarioRepoMock.Setup(r => r.ObterPorEmailAsync(admin.Email)).ReturnsAsync(admin);

        var resultado = await _service.LoginAsync(new LoginDto { Email = admin.Email, Senha = "123456" });

        resultado.Should().NotBeNull();
        resultado!.Usuario.Perfil.Should().Be("Administrador");
        resultado.Usuario.PerfilId.Should().Be(1);
    }
}
