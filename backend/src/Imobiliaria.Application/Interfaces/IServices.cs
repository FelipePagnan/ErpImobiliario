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

// --- V2 ---

public interface IVisitaService
{
    Task<IEnumerable<VisitaDto>> ObterTodasAsync();
    Task<VisitaDto?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<VisitaDto>> ObterPorClienteAsync(Guid clienteId);
    Task<IEnumerable<VisitaDto>> ObterPorCorretorAsync(Guid corretorId);
    Task<VisitaDto> CriarAsync(VisitaCreateDto dto);
    Task<VisitaDto?> AtualizarAsync(Guid id, VisitaUpdateDto dto);
    Task<bool> CancelarAsync(Guid id);
}

public interface IContratoService
{
    Task<IEnumerable<ContratoDto>> ObterTodosAsync();
    Task<ContratoDto?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<ContratoDto>> ObterVencendoAsync(int dias = 30);
    Task<ContratoDto> CriarAsync(ContratoCreateDto dto);
    Task<ContratoDto?> AtualizarAsync(Guid id, ContratoUpdateDto dto);
    Task<ContratoDto?> RescindirAsync(Guid id, string? motivo);
    Task<ContratoDto?> RenovarAsync(Guid id, DateTime novaDataFim, decimal? novoValor);
}

public interface IFinanceiroService
{
    Task<ResumoFinanceiroDto> ObterResumoAsync();
    Task<IEnumerable<LancamentoDto>> ObterLancamentosAsync();
    Task<IEnumerable<LancamentoDto>> ObterPorPeriodoAsync(DateTime inicio, DateTime fim);
    Task<LancamentoDto> CriarLancamentoAsync(LancamentoCreateDto dto);
    Task<LancamentoDto?> AtualizarLancamentoAsync(Guid id, LancamentoUpdateDto dto);
    Task<LancamentoDto?> PagarLancamentoAsync(Guid id);
    Task<IEnumerable<ComissaoDto>> ObterComissoesAsync();
    Task<ComissaoDto> CriarComissaoAsync(ComissaoCreateDto dto);
    Task<ComissaoDto?> PagarComissaoAsync(Guid id);
}

public interface ICrmService
{
    Task<IEnumerable<InteressadoDto>> ObterTodosAsync();
    Task<InteressadoDto?> ObterPorIdAsync(Guid id);
    Task<InteressadoDto> CriarAsync(InteressadoCreateDto dto);
    Task<InteressadoDto?> AtualizarAsync(Guid id, InteressadoUpdateDto dto);
    Task<bool> RemoverAsync(Guid id);
    Task<InteressadoDto?> RegistrarContatoAsync(Guid id, ContatoRegistroDto dto);
    Task<IEnumerable<ImovelDto>> ObterImoveisCompativeisAsync(Guid interessadoId);
}
