# Piramide de Testes - Minhas Financas

Este repositorio foi organizado para validar a aplicacao em tres niveis: regra de negocio, contrato da API e comportamento da interface. A ideia foi montar uma piramide de testes equilibrada, com mais testes baratos e rapidos na base, menos testes caros no topo, e cobertura suficiente para apontar falhas reais do sistema.

## Como rodar cada tipo de teste

### 1. Testes unitarios do backend (.NET)

Cobrem regras isoladas de dominio, sem depender de API, banco ou navegador.

```powershell
cd c:\Automação\CasosDeTeste
dotnet test backend-tests/MinhasFinancas.UnitTests/MinhasFinancas.UnitTests.csproj --configuration Release
```

### 2. Testes de integracao da API (.NET)

Validam o contrato real exposto no Swagger que esta rodando em http://localhost:5000.

```powershell
cd c:\Automação\CasosDeTeste
dotnet test backend-tests/MinhasFinancas.IntegrationTests/MinhasFinancas.IntegrationTests.csproj --configuration Release
```

### 3. Testes unitarios do frontend (React/TypeScript)

Validam comportamento de componente e formatacao sem depender do browser completo.

```powershell
cd c:\Automação\CasosDeTeste\frontend-tests
npm run test:unit
```

### 4. Testes end-to-end do frontend (Playwright)

Validam navegacao e comportamento da aplicacao rodando de verdade em http://localhost:5173.

```powershell
cd c:\Automação\CasosDeTeste\frontend-tests
npx playwright install chromium
npm run test:e2e
```

### 5. Rodar tudo de uma vez

```powershell
cd c:\Automação\CasosDeTeste
dotnet test MinhasFinancas.Testes.sln --configuration Release

cd c:\Automação\CasosDeTeste\frontend-tests
npm run test:unit
npm run test:e2e
```

## Como a piramide foi estruturada

Eu separei a piramide em tres camadas:

1. Base: testes unitarios.
Aqui ficam os testes mais baratos, rapidos e estaveis. No backend eles validam as regras do resumo financeiro. No frontend eles validam o componente de KPI e a formatacao de moeda.

2. Meio: testes de integracao.
Essa camada verifica se a API realmente entrega o que o Swagger promete. Foi aqui que concentrei a validacao de sucesso e de falha documentada para todas as operacoes expostas.

3. Topo: testes end-to-end.
Esses testes simulam o uso da aplicacao no navegador. Eles sao mais caros e mais sensiveis, entao ficaram focados em fluxos principais e em alguns cenarios negativos relevantes.

Estrutura criada no workspace:

- backend-tests/MinhasFinancas.Core: regras de dominio usadas pelos testes unitarios do backend.
- backend-tests/MinhasFinancas.UnitTests: testes unitarios com xUnit e FluentAssertions.
- backend-tests/MinhasFinancas.IntegrationTests: testes de integracao cobrindo todas as operacoes do Swagger.
- frontend-tests: testes unitarios do frontend com Vitest e testes E2E com Playwright.

## Bugs encontrados

Durante a execucao das suites, apareceram falhas reais de comportamento e de contrato.

### API de transacoes

- GET /api/v1/Transacoes retornou 500 quando o contrato esperado no Swagger e 200.
Regra que falhou: a listagem de transacoes deveria responder normalmente para consulta simples.

- POST /api/v1/Transacoes retornou 500 com payload valido quando o contrato esperado e 201.
Regra que falhou: a criacao de uma transacao valida deveria funcionar e devolver o recurso criado.

- GET /api/v1/Transacoes/{id} retornou 500 para item inexistente quando o Swagger documenta 404.
Regra que falhou: recurso inexistente deveria ser tratado como ausencia de dado, nao como erro interno.

### API de pessoas

- DELETE /api/v1/Pessoas/{id} retornou 204 para ID inexistente quando o Swagger documenta 404.
Regra que falhou: aqui existe divergencia entre implementacao e contrato. Ou a API deve responder 404, ou o Swagger deve assumir delete idempotente com 204.

### Frontend

- Ao acessar uma rota inexistente, a aplicacao perde o shell principal.
Regra que falhou: mesmo em rota invalida, a interface deveria manter o layout base e apresentar um fallback consistente.

Resumo do estado atual das suites:

- Unitarios .NET: 6 testes, todos passando.
- Integracao .NET: 22 testes executados, 17 passando e 5 falhando.
- Unitarios frontend: 4 testes, todos passando.
- E2E frontend: 4 testes executados, 3 passando e 1 falhando.

## Justificativa das escolhas de teste

As escolhas foram feitas para equilibrar custo, velocidade e capacidade de encontrar defeitos reais.

### Por que testes unitarios no backend

Usei testes unitarios para validar regra de negocio pura, porque eles sao os mais rapidos e baratos de manter. Eles ajudam a proteger calculos e validacoes basicas sem depender de infraestrutura externa.

### Por que testes de integracao focados no Swagger

Como a API ja estava rodando e exposta por Swagger, a forma mais objetiva de validar o sistema era testar o contrato real publicado. Isso evita suposicoes sobre implementacao interna e deixa evidente quando o comportamento da API nao bate com a documentacao.

### Por que testes unitarios no frontend

No frontend, preferi usar testes de componente para validar apresentacao e formatacao porque isso reduz custo e torna o feedback rapido. Esse tipo de teste e suficiente para pegar erros pequenos antes de subir para E2E.

### Por que poucos testes E2E e mais diretos

E2E e a camada mais cara da piramide. Por isso os testes ficaram concentrados em navegacao principal, comportamento quando a API falha e tratamento de rota invalida. Isso aumenta o valor do teste sem inflar manutencao com cenarios redundantes.

## Documentacao complementar

- Cobertura detalhada do Swagger: docs/swagger-test-coverage.md
- Relatorio consolidado de falhas: docs/test-failures-report.md