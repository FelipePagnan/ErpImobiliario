# 🏠 ERP Imobiliário + Portal Web

Sistema **ERP Imobiliário** desenvolvido com **.NET 8** e **React**, composto por uma API REST e um portal web para gerenciamento completo de imóveis, clientes, visitas, contratos, financeiro e relacionamento com interessados (CRM).

O projeto foi desenvolvido utilizando **Clean Architecture**, promovendo a separação de responsabilidades entre domínio, aplicação, infraestrutura e apresentação, além de autenticação via JWT, persistência com SQLite e documentação automática da API utilizando Swagger.

---

# 💻 Funcionalidades

### Portal Público
- Listagem de imóveis disponíveis
- Visualização detalhada dos imóveis
- Pesquisa e filtros avançados
- Sistema de login
- Cadastro de usuários

### Gestão Imobiliária
- Cadastro, edição e exclusão de imóveis
- Gestão de clientes
- Dashboard administrativo
- Controle de disponibilidade dos imóveis
- Controle de acesso por perfil

### CRM
- Cadastro de interessados
- Registro de contatos
- Preferências de imóveis
- Compatibilidade entre imóveis e interessados
- Sistema de favoritos

### Gestão de Visitas
- Agendamento de visitas
- Listagem de visitas
- Marcação como realizada
- Cancelamento de visitas

### Gestão de Contratos
- Criação de contratos
- Renovação de contratos
- Rescisão de contratos
- Atualização automática do status do imóvel

### Financeiro
- Controle de lançamentos
- Registro de pagamentos
- Gestão de comissões
- Resumo financeiro

### API
- API REST documentada com Swagger
- Autenticação JWT
- Autorização baseada em perfis (Roles)
- Endpoints para todos os módulos do sistema

---

# 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture**, separando regras de negócio, casos de uso, acesso aos dados e apresentação.

```text
ERP-Imobiliario
│
├── backend
│   ├── Imobiliaria.Domain
│   ├── Imobiliaria.Application
│   ├── Imobiliaria.Infrastructure
│   └── Imobiliaria.API
│
└── frontend
    ├── components
    ├── contexts
    ├── pages
    ├── services
    └── styles
```

## Responsabilidades

### Domain

Camada responsável pelo núcleo da aplicação.

- Entidades
- Enums
- Interfaces
- Regras de negócio

### Application

Responsável pelos casos de uso da aplicação.

- DTOs
- Services
- Interfaces de serviços

### Infrastructure

Responsável pelo acesso aos dados.

- Entity Framework Core
- SQLite
- Repositórios
- DbContext
- Seed inicial

### API

Camada responsável pela exposição dos serviços.

- Controllers REST
- Swagger
- JWT Authentication
- Injeção de Dependência

### Frontend

Portal desenvolvido em React.

- Portal público
- Área administrativa
- Dashboard
- Consumo da API via Axios

---

# 🛠️ Tecnologias Utilizadas

### Backend

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- JWT Authentication
- Swagger / OpenAPI

### Frontend

- React 18
- JavaScript
- React Router
- Axios
- CSS

### Arquitetura

- Clean Architecture
- Repository Pattern
- Dependency Injection

---

# 📂 Principais Recursos

### Gestão de Imóveis

Gerenciamento completo de imóveis com cadastro, atualização, filtros e controle de disponibilidade.

### CRM

Controle de interessados, favoritos, contatos e preferências de imóveis.

### Contratos

Gerenciamento do ciclo completo dos contratos, incluindo renovação e rescisão com atualização automática do imóvel.

### Financeiro

Controle de lançamentos financeiros, pagamentos e comissões.

### Portal Web

Área pública para consulta de imóveis e painel administrativo para gerenciamento do ERP.

### Segurança

Autenticação JWT e autorização baseada em perfis para controle de acesso às funcionalidades administrativas.

---

# 🚀 Como Executar

## Pré-requisitos

- .NET 8 SDK
- Node.js 18+

## 1. Backend

```bash
cd backend

dotnet restore

cd src/Imobiliaria.API

dotnet run
```

A API estará disponível em:

```
http://localhost:5000
```

Swagger:

```
http://localhost:5000/swagger
```

O banco SQLite é criado automaticamente na primeira execução juntamente com os dados de demonstração.

---

## 2. Frontend

```bash
cd frontend

npm install

npm start
```

O portal estará disponível em:

```
http://localhost:3000
```

---

# 👥 Contas de Demonstração

| Perfil | E-mail | Senha |
|---------|---------|--------|
| Administrador | admin@imobiliaria.com | admin123 |
| Gerente | gerente@imobiliaria.com | gerente123 |
| Corretor | carlos@imobiliaria.com | corretor123 |
| Corretor | ana@imobiliaria.com | corretor123 |
| Cliente | joao@email.com | cliente123 |

---

# 🗄️ Banco de Dados

O sistema utiliza **SQLite** como banco de dados local.

Na primeira execução, o banco é criado automaticamente juntamente com dados de demonstração para facilitar os testes da aplicação, incluindo:

- Usuários
- Imóveis
- Clientes
- Dados iniciais do sistema

---

# 📦 Evolução do Projeto

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

## V2 (Beta) — Novidades

- ✅ Sistema de favoritos de imóveis
- ✅ Gestão completa de visitas
- ✅ Gestão de contratos (criação, renovação e rescisão)
- ✅ Controle financeiro (lançamentos e pagamentos)
- ✅ Gestão de comissões imobiliárias
- ✅ CRM para interessados e contatos
- ✅ Compatibilidade entre imóveis e interessados
- ✅ Novas páginas administrativas (Visitas, Contratos, Financeiro e CRM)
- ✅ Navegação administrativa expandida
- ✅ Novos serviços e regras de negócio
- ✅ Novos endpoints REST documentados no Swagger

## V3 (Beta) — Extras

- ✅ Fotos nos imóveis
- ✅ CRUD de imóveis no painel
- ✅ Portal do Cliente
- ✅ Auditoria
- ✅ Dashboard com gráficos
- ✅ Melhorias visuais

---

# 📈 Roadmap

- [ ] Upload de imagens para imóveis
- [ ] Upload de documentos de contratos
- [ ] Relatórios em PDF
- [ ] Dashboard com gráficos
- [ ] Notificações por e-mail
- [ ] Docker
- [ ] Migração para SQL Server
- [ ] Testes automatizados

---

# 👨‍💻 Autor

**Felipe Pagnan**

Software Engineer especializado em desenvolvimento .NET, arquitetura de software e aplicações web.

**LinkedIn**

https://www.linkedin.com/in/felipe-pagnan/

---

# 📄 Licença

Este projeto está sob a licença **Pagnan**.

Sinta-se à vontade para estudar, utilizar como referência e contribuir com melhorias.
