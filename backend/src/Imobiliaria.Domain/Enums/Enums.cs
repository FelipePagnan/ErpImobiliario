namespace Imobiliaria.Domain.Enums;

public enum TipoImovel
{
    Casa = 1,
    Apartamento = 2,
    Cobertura = 3,
    Studio = 4,
    Kitnet = 5,
    Sobrado = 6,
    Terreno = 7,
    Chacara = 8,
    Fazenda = 9,
    Galpao = 10,
    SalaComercial = 11,
    Loja = 12,
    AreaIndustrial = 13
}

public enum FinalidadeImovel
{
    Venda = 1,
    Locacao = 2,
    VendaELocacao = 3,
    Troca = 4
}

public enum StatusImovel
{
    Disponivel = 1,
    Alugado = 2,
    Vendido = 3,
    EmAnalise = 4,
    Indisponivel = 5,
    EmReforma = 6
}

public enum PerfilUsuario
{
    Administrador = 1,
    Gerente = 2,
    Corretor = 3,
    Funcionario = 4,
    Cliente = 5
}

public enum StatusContrato
{
    Ativo = 1,
    Encerrado = 2,
    Rescindido = 3,
    EmAnalise = 4,
    Renovado = 5
}

public enum StatusVisita
{
    Solicitada = 1,
    Agendada = 2,
    Realizada = 3,
    Cancelada = 4,
    NaoCompareceu = 5
}
