# 💳 GerenciadorDeCartoes

API REST para gerenciamento de cartões de crédito e débito, com controle de **limite de crédito**, **saldo de débito**, **transações** e um **dashboard** com os totais do usuário. Desenvolvida em **.NET 8** seguindo **Clean Architecture**, com autenticação **JWT** e cobertura de testes automatizados.

---

## 📌 Introdução

O **GerenciadorDeCartoes** permite ao usuário:

- Criar conta e autenticar com **token JWT**;
- Registrar cartões com **limite de crédito**, **saldo de débito** e **crédito disponível**;
- Registrar **transações** em débito ou crédito, respeitando as regras financeiras;
- Alterar o valor de uma transação (reverte o lançamento e reprocessa no cartão);
- Remover cartões e transações;
- Consultar um **dashboard** com o total de limite, crédito disponível e débito.

**Regras de negócio principais:**

- 💳 **Crédito**: a soma do valor gasto não pode ultrapassar o limite do cartão;
- 💵 **Débito**: o saldo de débito não pode ficar negativo;
- 📊 **Crédito disponível** = `CreditLimit - CreditBalance`.

### ✨ Funcionalidades

| Módulo | Recursos |
|---|---|
| Usuários | Cadastro, login, perfil, atualização, troca de senha, exclusão |
| Cartões | Cadastro, listagem, atualização, exclusão |
| Transações | Cadastro, listagem, alteração de valor, exclusão |
| Dashboard | Limite total, crédito disponível e débito somados |

---

## 🧰 Tecnologias

| Tecnologia | Finalidade |
|---|---|
| **.NET 8** / ASP.NET Core Web API | Framework da aplicação |
| **Entity Framework Core** + Pomelo MySQL | ORM / acesso a dados |
| **MySQL 8** | Banco de dados relacional |
| **FluentMigrator** + Dapper | Migrações e criação automática do banco |
| **FluentValidation** | Validação das requisições |
| **AutoMapper** | Mapeamento entre DTOs e entidades |
| **JWT** (JwtBearer + IdentityModel) | Autenticação e autorização |
| **SHA-512** | Criptografia de senhas (hash em hexadecimal) |
| **Swagger / OpenAPI** (Swashbuckle) | Documentação interativa da API |
| **xUnit**, FluentAssertions, Moq, Bogus, WebApplicationFactory | Testes unitários e de integração |
| **GitHub Actions** + **SonarCloud** | CI/CD e análise de qualidade de código |


---

## 🏗️ Arquitetura

O projeto segue **Clean Architecture**, separando as responsabilidades em camadas:

```
┌─────────────────────────────────────────────────────┐
│  CardManager.API            (Apresentação)          │
│  Controllers · Middleware · Filters · JWT · Swagger │
├─────────────────────────────────────────────────────┤
│  CardManager.Application     (Aplicação)            │
│  Use Cases · Validators · Services · AutoMapper     │
├─────────────────────────────────────────────────────┤
│  CardManager.Domain          (Domínio)              │
│  Entities · Repository Interfaces · Security        │
├─────────────────────────────────────────────────────┤
│  CardManager.Infrastructure  (Infraestrutura)       │
│  EF Core · Repositories · Migrations · JwtToken     │
└─────────────────────────────────────────────────────┘
    CardManager.Communication (Shared) · CardManager.Exceptions (Shared)
```

### Estrutura de pastas

```
├── src/
│   ├── Backend/
│   │   ├── CardManager.API/            # Camada de apresentação
│   │   ├── CardManager.Application/    # Use cases e regras de negócio
│   │   ├── CardManager.Domain/         # Entidades e interfaces
│   │   └── CardManager.Infrastructure/ # Acesso a dados e migrações
│   └── Shared/
│       ├── CardManager.Communication/  # Requests/Responses/Enums
│       └── CardManager.Exceptions/     # Exceções e mensagens
├── tests/
│   ├── Api.Test/                       # Testes de integração
│   ├── UseCases.Test/                  # Testes de use cases
│   ├── Validators.Test/                # Testes de validação
│   └── CoreTestUtilities/              # Builders e mocks compartilhados
└── CardManager.sln
```

---

## ✅ Pré-requisitos

- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** (ou superior);
- **[MySQL 8](https://dev.mysql.com/downloads/)** instalado e em execução;
- **[Git](https://git-scm.com/)** para clonar o repositório;
- Um usuário do MySQL com permissão para criar bancos de dados (ex.: `root`).

> 💡 Os testes de integração usam um banco **InMemory** (`EF Core InMemory`), portanto não exigem o MySQL rodando.

---

## 🚀 Como Rodar

### 1. Clonar o repositório

```
git clone https://github.com/wellingtonbento/GerenciadorDeCartoes.git
cd GerenciadorDeCartoes
```
###2. Configurar as credenciais do banco
```
Edite o arquivo src/Backend/CardManager.API/appsettings.Development.json e preencha com as suas credenciais:
{
  "ConnectionStrings": {
    "Connection": "Server=localhost;Database=gerenciamentodecartoes;Uid=root;Pwd=<TROQUE_PELA_SUA_SENHA>"
  },
  "Jwt": {
    "SigninKey": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
    "ExpirationTokenInMunites": 500
  }
}
```
⚠️ Importante: substitua <TROQUE_PELA_SUA_SENHA> pela senha do seu MySQL.

---

## 📡 Endpoints da API

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| `POST` | `/authentication` | ❌ | Login e geração de token JWT |
| `POST` | `/users` | ❌ | Cadastro de usuário |
| `GET` | `/users` | ✅ | Perfil do usuário logado |
| `PUT` | `/users` | ✅ | Atualizar nome/e-mail |
| `PATCH` | `/users/password` | ✅ | Alterar senha |
| `DELETE` | `/users` | ✅ | Remover conta |
| `POST` | `/cards` | ✅ | Registrar cartão |
| `GET` | `/cards` | ✅ | Listar cartões |
| `PUT` | `/cards/{cardId}` | ✅ | Atualizar cartão |
| `DELETE` | `/cards/{cardId}` | ✅ | Remover cartão |
| `POST` | `/transactions` | ✅ | Registrar transação |
| `GET` | `/transactions/{cardId}` | ✅ | Listar transações de um cartão |
| `PATCH` | `/transactions/{cardId}/{transactionId}/amount` | ✅ | Alterar valor da transação |
| `DELETE` | `/transactions/{cardId}/{transactionId}` | ✅ | Remover transação |
| `GET` | `/dashboard` | ✅ | Totais do usuário (limite, crédito disponível, débito) |

---

## 🧪 Testes


- **Api.Test** — testes de integração com `WebApplicationFactory` e banco InMemory;
- **UseCases.Test** — testes dos use cases com mocks (Moq);
- **Validators.Test** — testes das validações do FluentValidation.

---

## 🤖 CI/CD

O repositório possui **GitHub Actions** configurado com o **SonarCloud**:

- Dispara no push para a branch `develop`;
- Realiza `build` e `dotnet test` com cobertura (OpenCover);
- Envia a análise para o SonarCloud para acompanhamento de qualidade de código.
