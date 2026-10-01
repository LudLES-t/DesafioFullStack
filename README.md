# Desafio Full Stack

Aplicação de gerenciamento de tarefas com backend em .NET 8 e frontend em SAPUI5.

## Tecnologias

- Backend: ASP.NET Core Web API
- Banco de dados: SQLite
- Frontend: SAPUI5
- Testes: xUnit + ASP.NET Core TestHost

## Estrutura do projeto

- `backend/TodoApi` - API REST
- `backend/TodoApi.Tests` - testes de integração
- `frontend/webapp` - aplicação frontend

## Pré-requisitos

- .NET 8 SDK
- Navegador moderno
- Python 3 (opcional, para servir o frontend localmente)

## Executando o backend

No terminal, na raiz do projeto:

```bash
cd backend/TodoApi
dotnet restore
dotnet run
```

A API ficará disponível em:

- `http://localhost:5066`

Endpoints principais:

- `GET /todos?page=1&pageSize=10`
- `GET /todos/{id}`
- `POST /todos`
- `PUT /todos/{id}`
- `DELETE /todos/{id}`

## Executando o frontend

Você pode abrir o arquivo `frontend/webapp/index.html` em um servidor local.

Exemplo usando Python:

```bash
cd frontend
python3 -m http.server 8000
```

Depois acesse:

```text
http://localhost:8000
```

## Executando testes

```bash
cd backend/TodoApi.Tests
dotnet test
```

## Observações

- O backend usa SQLite e cria o banco automaticamente em execução.
- A aplicação foi organizada para funcionar como um desafio Full Stack, com API e interface em SAPUI5.

## Repositório

- GitHub: https://github.com/LudLES-t/DesafioFullStack
