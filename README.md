# ERP Imobiliário + Portal Web

Sistema ERP Imobiliário completo com API em .NET 8 e portal web em React.

## Tecnologias

**Backend:** .NET 8, ASP.NET Core Web API, Entity Framework Core, SQLite, JWT  
**Frontend:** React 18, JavaScript, React Router, Axios  
**Arquitetura:** Clean Architecture (Domain, Application, Infrastructure, API)

---

## Como Rodar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)

### 1. Backend (API)

```bash
cd backend

# Restaurar pacotes
dotnet restore

# Rodar a API (porta 5000)
cd src/Imobiliaria.API
dotnet run
```

A API estará disponível em: `http://localhost:5000`  
Swagger UI: `http://localhost:5000/swagger`

O banco SQLite (`imobiliaria.db`) é criado automaticamente na primeira execução com dados de exemplo.

### 2. Frontend (React)

```bash
cd frontend

# Instalar dependências
npm install

# Rodar (porta 3000)
npm start
```

O portal estará disponível em: `http://localhost:3000`

---

## Contas de Demonstração

| Perfil        | E-mail                     | Senha        |
|---------------|----------------------------|--------------|
| Administrador | admin@imobiliaria.com      | admin123     |
| Gerente       | gerente@imobiliaria.com    | gerente123   |
| Corretor      | carlos@imobiliaria.com     | corretor123  |
| Corretor      | ana@imobiliaria.com        | corretor123  |
| Cliente       | joao@email.com             | cliente123   |

---

## Estrutura do Projeto

```
ERP-Imobiliario/
├── backend/
│   ├── Imobiliaria.sln
│   └── src/
│       ├── Imobiliaria.Domain/          # Entidades, Enums, Interfaces
│       ├── Imobiliaria.Application/     # DTOs, Services, Interfaces
│       ├── Imobiliaria.Infrastructure/  # DbContext, Repositories, Seed
│       └── Imobiliaria.API/             # Controllers, Program.cs
└── frontend/
    ├── public/
    └── src/
        ├── components/    # Header, Footer, PropertyCard
        ├── contexts/      # AuthContext
        ├── pages/         # Home, PropertyList, PropertyDetail, Login, Dashboard
        ├── services/      # api.js (Axios)
        └── styles/        # CSS variables e global
```

---

## API Endpoints

### Auth
- `POST /api/auth/login` — Login (retorna JWT)
- `POST /api/auth/registrar` — Registrar novo usuário

### Imóveis
- `GET /api/imoveis` — Listar todos
- `GET /api/imoveis/{id}` — Obter por ID
- `GET /api/imoveis/filtrar?cidade=&tipo=&finalidade=` — Filtrar
- `GET /api/imoveis/dashboard` — Dados do dashboard (autenticado)
- `POST /api/imoveis` — Criar (Admin/Gerente)
- `PUT /api/imoveis/{id}` — Atualizar (Admin/Gerente/Corretor)
- `DELETE /api/imoveis/{id}` — Remover (Admin)

---

## Dados de Exemplo (Seed)

O sistema já vem com 8 imóveis cadastrados em Maringá/PR:

1. Casa com piscina — Venda R$ 850.000
2. Apartamento 2 quartos — Locação R$ 2.200/mês
3. Sobrado condomínio fechado — Venda R$ 620.000
4. Cobertura duplex — Venda R$ 1.200.000 / Locação R$ 6.500/mês
5. Kitnet centro — Locação R$ 800/mês
6. Terreno 450m² — Venda R$ 280.000
7. Sala comercial — Locação R$ 1.800/mês
8. Casa 2 quartos (alugada) — Locação R$ 1.400/mês

---

## V1 (Beta) — O que está incluso

- ✅ Clean Architecture completa
- ✅ CRUD de imóveis via API
- ✅ Autenticação JWT
- ✅ Controle de permissão por perfil (roles)
- ✅ Filtros avançados de imóveis
- ✅ Seed data com imóveis de exemplo
- ✅ Portal público (Home + Listagem + Detalhe)
- ✅ Página de login com contas demo
- ✅ Dashboard administrativo
- ✅ Layout responsivo
- ✅ Swagger configurado

## Próximas versões (planejado)

- Módulo de Contratos
- Módulo Financeiro
- CRM (favoritos, preferências, notificações)
- Gestão de Visitas
- Upload de fotos e documentos
- Auditoria
- Relatórios
- Portal do Cliente separado
