using Imobiliaria.Application.DTOs;

namespace Imobiliaria.Application.Interfaces;

public interface IImovelService
{
    Task<IEnumerable<ImovelDto>> ObterTodosAsync();
    Task<ImovelDto?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<ImovelDto>> FiltrarAsync(ImovelFilterDto filtro);
    Task<ImovelDto> CriarAsync(ImovelCreateDto dto);
    Task<ImovelDto?> AtualizarAsync(Guid id, ImovelUpdateDto dto);
    Task<bool> RemoverAsync(Guid id);
    Task<DashboardDto> ObterDashboardAsync();
}

public interface IAuthService
{
    Task<TokenDto?> LoginAsync(LoginDto dto);
    Task<TokenDto?> RegistrarAsync(RegisterDto dto);
}

public interface IFavoritoService
{
    Task<IEnumerable<ImovelDto>> ObterFavoritosAsync(Guid clienteId);
    Task<bool> AdicionarFavoritoAsync(Guid clienteId, Guid imovelId);
    Task<bool> RemoverFavoritoAsync(Guid clienteId, Guid imovelId);
    Task<bool> EhFavoritoAsync(Guid clienteId, Guid imovelId);
}
