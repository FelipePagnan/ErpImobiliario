using System.Security.Cryptography;
using System.Text;
using Imobiliaria.Domain.Entities;
using Imobiliaria.Domain.Enums;
using Imobiliaria.Infrastructure.Data;

namespace Imobiliaria.Infrastructure.Seed;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();
        if (context.Usuarios.Any()) return;

        // ===== USUÁRIOS =====
        var admin = new Usuario { Nome = "Admin Sistema", Email = "admin@imobiliaria.com", SenhaHash = HashSenha("admin123"), Perfil = PerfilUsuario.Administrador };
        var gerente = new Usuario { Nome = "Maria Gerente", Email = "gerente@imobiliaria.com", SenhaHash = HashSenha("gerente123"), Perfil = PerfilUsuario.Gerente };
        var usuarioCorretor1 = new Usuario { Nome = "Carlos Silva", Email = "carlos@imobiliaria.com", SenhaHash = HashSenha("corretor123"), Perfil = PerfilUsuario.Corretor };
        var usuarioCorretor2 = new Usuario { Nome = "Ana Oliveira", Email = "ana@imobiliaria.com", SenhaHash = HashSenha("corretor123"), Perfil = PerfilUsuario.Corretor };
        var usuarioCliente = new Usuario { Nome = "João Cliente", Email = "joao@email.com", SenhaHash = HashSenha("cliente123"), Perfil = PerfilUsuario.Cliente };
        context.Usuarios.AddRange(admin, gerente, usuarioCorretor1, usuarioCorretor2, usuarioCliente);

        // ===== CORRETORES =====
        var corretor1 = new Corretor { Nome = "Carlos Silva", CRECI = "CRECI-12345/PR", Email = "carlos@imobiliaria.com", Telefone = "(44) 99999-1111", UsuarioId = usuarioCorretor1.Id };
        var corretor2 = new Corretor { Nome = "Ana Oliveira", CRECI = "CRECI-67890/PR", Email = "ana@imobiliaria.com", Telefone = "(44) 99999-2222", UsuarioId = usuarioCorretor2.Id };
        context.Corretores.AddRange(corretor1, corretor2);

        // ===== PROPRIETÁRIOS =====
        var prop1 = new Proprietario { Nome = "Roberto Souza", CPFouCNPJ = "123.456.789-00", Email = "roberto@email.com", Telefone = "(44) 98888-1111" };
        var prop2 = new Proprietario { Nome = "Fernanda Lima", CPFouCNPJ = "987.654.321-00", Email = "fernanda@email.com", Telefone = "(44) 98888-2222" };
        var prop3 = new Proprietario { Nome = "Construtora ABC Ltda", CPFouCNPJ = "12.345.678/0001-00", Email = "contato@construtoraabc.com", Telefone = "(44) 3333-4444" };
        context.Proprietarios.AddRange(prop1, prop2, prop3);

        // ===== CLIENTES =====
        var cliente1 = new Cliente { Nome = "João Cliente", CPFouCNPJ = "111.222.333-44", Email = "joao@email.com", Telefone = "(44) 97777-1111", UsuarioId = usuarioCliente.Id };
        context.Clientes.Add(cliente1);

        // ===== IMÓVEIS COM FOTOS =====

        var end1 = new Endereco { Logradouro = "Rua das Palmeiras", Numero = "450", Bairro = "Jardim América", Cidade = "Maringá", Estado = "PR", CEP = "87010-100" };
        var imovel1 = new Imovel
        {
            Titulo = "Casa Ampla com Piscina - Jardim América", Codigo = "CAS-001", Tipo = TipoImovel.Casa, Finalidade = FinalidadeImovel.Venda, Status = StatusImovel.Disponivel,
            Descricao = "Linda casa com 3 suítes, piscina aquecida, churrasqueira gourmet e jardim. Acabamento de alto padrão, porcelanato em todos os ambientes.",
            PrecoVenda = 850000m, ValorIPTU = 3200m, AreaTotal = 360, AreaConstruida = 220, Dormitorios = 3, Suites = 3, Banheiros = 4, VagasGaragem = 3,
            FotoPrincipalUrl = "https://picsum.photos/seed/casa-piscina/800/500",
            Endereco = end1, ProprietarioId = prop1.Id, CorretorId = corretor1.Id,
            CaracteristicasJson = "[\"Piscina\",\"Churrasqueira\",\"Jardim\",\"Porcelanato\",\"Aquecimento Solar\"]"
        };

        var end2 = new Endereco { Logradouro = "Avenida Brasil", Numero = "1200", Complemento = "Bloco B, Apto 304", Bairro = "Zona 7", Cidade = "Maringá", Estado = "PR", CEP = "87020-200" };
        var imovel2 = new Imovel
        {
            Titulo = "Apartamento Moderno - Zona 7", Codigo = "APT-001", Tipo = TipoImovel.Apartamento, Finalidade = FinalidadeImovel.Locacao, Status = StatusImovel.Disponivel,
            Descricao = "Apartamento de 2 dormitórios com sacada gourmet. Condomínio com academia, salão de festas e playground.",
            PrecoLocacao = 2200m, ValorCondominio = 450m, ValorIPTU = 1200m, AreaTotal = 75, AreaConstruida = 68, Dormitorios = 2, Suites = 1, Banheiros = 2, VagasGaragem = 1, Andar = 3, Andares = 12,
            FotoPrincipalUrl = "https://picsum.photos/seed/apto-moderno/800/500",
            Endereco = end2, ProprietarioId = prop2.Id, CorretorId = corretor1.Id,
            CaracteristicasJson = "[\"Sacada Gourmet\",\"Academia\",\"Salão de Festas\",\"Playground\",\"Portaria 24h\"]"
        };

        var end3 = new Endereco { Logradouro = "Rua Pioneiro Antonio Ruiz", Numero = "880", Bairro = "Jardim Alvorada", Cidade = "Maringá", Estado = "PR", CEP = "87030-300" };
        var imovel3 = new Imovel
        {
            Titulo = "Sobrado em Condomínio Fechado", Codigo = "SOB-001", Tipo = TipoImovel.Sobrado, Finalidade = FinalidadeImovel.Venda, Status = StatusImovel.Disponivel,
            Descricao = "Sobrado de alto padrão em condomínio fechado com segurança 24h. Área de lazer completa, espaço gourmet e quintal.",
            PrecoVenda = 620000m, ValorCondominio = 600m, ValorIPTU = 2800m, AreaTotal = 250, AreaConstruida = 180, Dormitorios = 3, Suites = 1, Banheiros = 3, VagasGaragem = 2, Andares = 2,
            FotoPrincipalUrl = "https://picsum.photos/seed/sobrado-cond/800/500",
            Endereco = end3, ProprietarioId = prop3.Id, CorretorId = corretor2.Id,
            CaracteristicasJson = "[\"Condomínio Fechado\",\"Segurança 24h\",\"Espaço Gourmet\",\"Quintal\"]"
        };

        var end4 = new Endereco { Logradouro = "Avenida Colombo", Numero = "5500", Complemento = "Torre A, Apto 1502", Bairro = "Zona 7", Cidade = "Maringá", Estado = "PR", CEP = "87020-900" };
        var imovel4 = new Imovel
        {
            Titulo = "Cobertura Duplex com Vista Panorâmica", Codigo = "COB-001", Tipo = TipoImovel.Cobertura, Finalidade = FinalidadeImovel.VendaELocacao, Status = StatusImovel.Disponivel,
            Descricao = "Cobertura duplex com vista panorâmica da cidade. 4 suítes, sala ampla, varanda gourmet com churrasqueira.",
            PrecoVenda = 1200000m, PrecoLocacao = 6500m, ValorCondominio = 1200m, ValorIPTU = 5000m, AreaTotal = 200, AreaConstruida = 200, Dormitorios = 4, Suites = 4, Banheiros = 5, VagasGaragem = 3, Andar = 15, Andares = 16, Mobiliado = true,
            FotoPrincipalUrl = "https://picsum.photos/seed/cobertura-luxo/800/500",
            Endereco = end4, ProprietarioId = prop3.Id, CorretorId = corretor1.Id,
            CaracteristicasJson = "[\"Vista Panorâmica\",\"Duplex\",\"Churrasqueira\",\"Mobiliado\",\"Piso Aquecido\"]"
        };

        var end5 = new Endereco { Logradouro = "Rua Neo Alves Martins", Numero = "1450", Complemento = "Apto 22", Bairro = "Zona 1", Cidade = "Maringá", Estado = "PR", CEP = "87013-060" };
        var imovel5 = new Imovel
        {
            Titulo = "Kitnet Centro - Ideal Estudantes", Codigo = "KIT-001", Tipo = TipoImovel.Kitnet, Finalidade = FinalidadeImovel.Locacao, Status = StatusImovel.Disponivel,
            Descricao = "Kitnet bem localizada no centro da cidade, próxima à UEM. Mobiliada com cama, guarda-roupa e fogão.",
            PrecoLocacao = 800m, AreaTotal = 28, AreaConstruida = 28, Dormitorios = 1, Banheiros = 1, VagasGaragem = 0, Andar = 2, Andares = 4, Mobiliado = true,
            FotoPrincipalUrl = "https://picsum.photos/seed/kitnet-centro/800/500",
            Endereco = end5, ProprietarioId = prop2.Id, CorretorId = corretor2.Id,
            CaracteristicasJson = "[\"Mobiliado\",\"Água Inclusa\",\"Gás Incluso\",\"Próximo UEM\"]"
        };

        var end6 = new Endereco { Logradouro = "Rua dos Ipês", Numero = "SN", Bairro = "Parque Residencial Cidade Nova", Cidade = "Maringá", Estado = "PR", CEP = "87023-000" };
        var imovel6 = new Imovel
        {
            Titulo = "Terreno 450m² - Excelente Localização", Codigo = "TER-001", Tipo = TipoImovel.Terreno, Finalidade = FinalidadeImovel.Venda, Status = StatusImovel.Disponivel,
            Descricao = "Terreno plano em bairro residencial com toda infraestrutura. Pronto para construir.",
            PrecoVenda = 280000m, ValorIPTU = 800m, AreaTotal = 450, Dormitorios = 0, Banheiros = 0, VagasGaragem = 0,
            FotoPrincipalUrl = "https://picsum.photos/seed/terreno-plano/800/500",
            Endereco = end6, ProprietarioId = prop1.Id, CorretorId = corretor2.Id,
            CaracteristicasJson = "[\"Terreno Plano\",\"Documentação OK\",\"Infraestrutura Completa\"]"
        };

        var end7 = new Endereco { Logradouro = "Avenida Tiradentes", Numero = "900", Complemento = "Sala 205", Bairro = "Zona 1", Cidade = "Maringá", Estado = "PR", CEP = "87013-260" };
        var imovel7 = new Imovel
        {
            Titulo = "Sala Comercial no Centro", Codigo = "SAL-001", Tipo = TipoImovel.SalaComercial, Finalidade = FinalidadeImovel.Locacao, Status = StatusImovel.Disponivel,
            Descricao = "Sala comercial em edifício empresarial com elevador e estacionamento.",
            PrecoLocacao = 1800m, ValorCondominio = 350m, AreaTotal = 45, AreaConstruida = 45, Dormitorios = 0, Banheiros = 1, VagasGaragem = 1, Andar = 2, Andares = 8,
            FotoPrincipalUrl = "https://picsum.photos/seed/sala-comercial/800/500",
            Endereco = end7, ProprietarioId = prop3.Id, CorretorId = corretor1.Id,
            CaracteristicasJson = "[\"Elevador\",\"Estacionamento\",\"Ar Condicionado\",\"Recepção\"]"
        };

        var end8 = new Endereco { Logradouro = "Rua Rio Branco", Numero = "330", Bairro = "Zona 5", Cidade = "Maringá", Estado = "PR", CEP = "87015-380" };
        var imovel8 = new Imovel
        {
            Titulo = "Casa 2 Quartos - Zona 5", Codigo = "CAS-002", Tipo = TipoImovel.Casa, Finalidade = FinalidadeImovel.Locacao, Status = StatusImovel.Alugado,
            Descricao = "Casa simples e funcional com 2 quartos, sala, cozinha e lavanderia. Quintal nos fundos.",
            PrecoLocacao = 1400m, ValorIPTU = 900m, AreaTotal = 150, AreaConstruida = 90, Dormitorios = 2, Banheiros = 1, VagasGaragem = 1,
            FotoPrincipalUrl = "https://picsum.photos/seed/casa-simples/800/500",
            Endereco = end8, ProprietarioId = prop1.Id, CorretorId = corretor2.Id,
            CaracteristicasJson = "[\"Quintal\",\"Lavanderia\"]"
        };

        context.Imoveis.AddRange(imovel1, imovel2, imovel3, imovel4, imovel5, imovel6, imovel7, imovel8);

        // ===== AUDITORIA INICIAL =====
        context.Set<Auditoria>().Add(new Auditoria { Acao = "SeedData", Entidade = "Sistema", Detalhes = "Dados iniciais carregados", UsuarioNome = "Sistema" });

        context.SaveChanges();
    }

    private static string HashSenha(string senha)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(senha)));
    }
}
