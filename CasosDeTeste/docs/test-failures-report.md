# Relatorio consolidado de falhas encontradas

## Escopo avaliado

As falhas abaixo foram encontradas durante a execucao das suites:

- unitarios .NET;
- integracao .NET baseada no Swagger;
- unitarios frontend com Vitest;
- end-to-end frontend com Playwright.

## Resumo por suite

| Suite | Status | Evidencia |
|---|---|---|
| Unitarios .NET | Passando | 6 de 6 testes verdes |
| Integracao .NET | Falhando | 5 falhas reais de contrato/comportamento |
| Unitarios frontend | Passando | 4 de 4 testes verdes |
| E2E frontend | Falhando | 1 falha real de navegacao |

## Falhas encontradas na integracao .NET

### 1. GET /api/v1/Transacoes retorna 500

- Esperado pelo Swagger: `200 OK`
- Observado na execucao: `500 Internal Server Error`
- Impacto: a listagem de transacoes quebra qualquer consumidor que dependa do contrato documentado.
- Consequencia para o frontend: a tela de transacoes fica presa em carregamento.

### 2. POST /api/v1/Transacoes retorna 500 com payload valido

- Esperado pelo Swagger: `201 Created`
- Observado na execucao: `500 Internal Server Error`
- Impacto: nao e possivel criar transacoes de forma confiavel.
- Consequencia indireta: testes dependentes de criacao de transacao tambem falham.

### 3. GET /api/v1/Transacoes/{id} retorna 500 para ID inexistente

- Esperado pelo Swagger: `404 Not Found`
- Observado na execucao: `500 Internal Server Error`
- Impacto: a API nao diferencia ausencia de recurso de erro interno.
- Consequencia: clientes nao conseguem tratar corretamente o caso de item inexistente.

### 4. DELETE /api/v1/Pessoas/{id} retorna 204 para ID inexistente

- Esperado pelo Swagger: `404 Not Found`
- Observado na execucao: `204 No Content`
- Impacto: existe divergencia entre implementacao e contrato.
- Tradeoff: se a intencao e DELETE idempotente, o Swagger precisa ser corrigido; se a intencao e aderir ao contrato atual, a API precisa mudar.

### 5. Falha encadeada nos testes de GET por ID de Transacoes

- Causa raiz: `POST /api/v1/Transacoes` falha com `500`.
- Efeito: o teste que cria uma transacao para depois buscá-la por ID nao consegue preparar o dado.
- Importante: esta nao e uma causa independente; e um efeito da falha numero 2.

## Falha encontrada no E2E frontend

### 6. Rota inexistente derruba o shell principal da aplicacao

- Teste: acesso a `/rota-inexistente`
- Esperado no teste: manter o chrome basico da aplicacao visivel, com cabecalho e navegacao.
- Observado na execucao: o texto `Minhas Finanças` nao fica visivel, indicando perda do shell principal.
- Impacto: experiencia ruim em navegacao invalida e ausencia de fallback de rota.
- Possivel causa: falta de rota catch-all (`*`) ou tratamento de not found no router.

## Falhas nao encontradas nas outras suites

### Unitarios .NET

Nenhuma falha encontrada. Os cenarios abaixo passaram:

- calculo de saldo com receitas e despesas;
- separacao de totais;
- rejeicao de valor invalido;
- colecao vazia;
- tratamento de argumento nulo.

### Unitarios frontend

Nenhuma falha encontrada. Os cenarios abaixo passaram:

- formatacao monetaria positiva;
- formatacao monetaria negativa;
- renderizacao com icone;
- renderizacao sem icone mantendo acessibilidade.

## Arquivos relacionados

- `backend-tests/MinhasFinancas.IntegrationTests/UnitTest1.cs`
- `backend-tests/MinhasFinancas.UnitTests/UnitTest1.cs`
- `frontend-tests/src/components/KpiCard.test.tsx`
- `frontend-tests/e2e/app-navigation.spec.ts`
- `docs/swagger-test-coverage.md`

## Como reproduzir

### Integracao .NET

```powershell
cd c:\Automação\CasosDeTeste
dotnet test backend-tests/MinhasFinancas.IntegrationTests/MinhasFinancas.IntegrationTests.csproj --configuration Release
```

### Unitarios .NET

```powershell
cd c:\Automação\CasosDeTeste
dotnet test backend-tests/MinhasFinancas.UnitTests/MinhasFinancas.UnitTests.csproj --configuration Release
```

### Frontend

```powershell
cd c:\Automação\CasosDeTeste\frontend-tests
npm run test:unit
npm run test:e2e
```
