# Cobertura de testes do Swagger

## Objetivo

Garantir cobertura automatizada para todas as operacoes documentadas em `http://localhost:5000/swagger/index.html`, incluindo:

- fluxo de sucesso para todas as operacoes do Swagger;
- fluxo de falha para operacoes que documentam erro no contrato (`400` ou `404`).

## Matriz de cobertura

| Endpoint | Metodo | Teste de sucesso | Teste de falha | Status observado |
|---|---|---|---|---|
| /api/v1/Categorias | GET | Sim | Nao documentado no Swagger | Passando |
| /api/v1/Categorias | POST | Sim | Sim (`400`) | Passando |
| /api/v1/Categorias/{id} | GET | Sim | Sim (`404`) | Passando |
| /api/v1/Pessoas | GET | Sim | Nao documentado no Swagger | Passando |
| /api/v1/Pessoas | POST | Sim | Sim (`400`) | Passando |
| /api/v1/Pessoas/{id} | GET | Sim | Sim (`404`) | Passando |
| /api/v1/Pessoas/{id} | PUT | Sim | Sim (`400`) | Passando |
| /api/v1/Pessoas/{id} | DELETE | Sim | Sim (`404`) | Falhando |
| /api/v1/Totais/pessoas | GET | Sim | Nao documentado no Swagger | Passando |
| /api/v1/Totais/categorias | GET | Sim | Nao documentado no Swagger | Passando |
| /api/v1/Transacoes | GET | Sim | Nao documentado no Swagger | Falhando |
| /api/v1/Transacoes | POST | Sim | Sim (`400`) | Falhando no sucesso |
| /api/v1/Transacoes/{id} | GET | Sim | Sim (`404`) | Falhando |

## O que os testes provaram

### Contratos atendidos

- Todas as operacoes de `Categorias` respeitam o contrato do Swagger.
- Todas as operacoes de `Pessoas`, exceto o `DELETE` de falha, respeitam o contrato documentado.
- Os endpoints de `Totais` respondem corretamente em cenarios de sucesso.

### Quebras de contrato encontradas

1. `GET /api/v1/Transacoes` retorna `500`, mas o Swagger documenta `200`.
2. `POST /api/v1/Transacoes` retorna `500` com payload valido, mas o Swagger documenta `201`.
3. `GET /api/v1/Transacoes/{id}` retorna `500` mesmo para ID inexistente, mas o Swagger documenta `404`.
4. `DELETE /api/v1/Pessoas/{id}` retorna `204` para ID inexistente, mas o Swagger documenta `404`.

## Local dos testes

A cobertura do Swagger foi implementada em:

- `backend-tests/MinhasFinancas.IntegrationTests/UnitTest1.cs`

## Como executar

```powershell
cd c:\Automação\CasosDeTeste
dotnet test backend-tests/MinhasFinancas.IntegrationTests/MinhasFinancas.IntegrationTests.csproj --configuration Release
```

## Observacoes importantes

- Os testes de sucesso de `POST` criam dados reais no ambiente local, porque o Swagger exposto nao oferece operacoes de limpeza para `Categorias` e `Transacoes`.
- Os nomes enviados nos payloads sao unicos por timestamp para reduzir colisao entre execucoes.
- Nos endpoints onde o Swagger nao documenta resposta de falha, o teste cobre apenas o contrato de sucesso; ainda assim, falhas reais de runtime aparecem como erro da suite.
