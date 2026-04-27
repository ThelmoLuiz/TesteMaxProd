# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: app-navigation.spec.ts >> Navegacao principal >> deve nao quebrar o chrome da aplicacao em rota inexistente
- Location: e2e\app-navigation.spec.ts:46:3

# Error details

```
Error: expect(locator).toBeVisible() failed

Locator: getByText('Minhas Finanças')
Expected: visible
Timeout: 8000ms
Error: element(s) not found

Call log:
  - Expect "toBeVisible" with timeout 8000ms
  - waiting for getByText('Minhas Finanças')

```

# Test source

```ts
  1  | import { expect, test } from "@playwright/test";
  2  | 
  3  | test.describe("Navegacao principal", () => {
  4  |   test("deve abrir dashboard e exibir shell da aplicacao", async ({ page }) => {
  5  |     await page.goto("/");
  6  | 
  7  |     await expect(page.getByText("Minhas Finanças")).toBeVisible();
  8  |     await expect(page.getByRole("link", { name: "Dashboard" }).first()).toBeVisible();
  9  |     await expect(page.getByRole("link", { name: "Transações" }).first()).toBeVisible();
  10 |     await expect(page.getByRole("link", { name: "Categorias" }).first()).toBeVisible();
  11 |     await expect(page.getByRole("link", { name: "Relatórios" }).first()).toBeVisible();
  12 |   });
  13 | 
  14 |   test("deve navegar para paginas principais", async ({ page }) => {
  15 |     await page.goto("/");
  16 | 
  17 |     await page.getByRole("link", { name: "Transações" }).first().click();
  18 |     await expect(page).toHaveURL(/\/transacoes$/);
  19 |     await expect(page.getByRole("link", { name: "Transacoes" })).toBeVisible();
  20 | 
  21 |     await page.getByRole("link", { name: "Categorias" }).first().click();
  22 |     await expect(page).toHaveURL(/\/categorias$/);
  23 |     await expect(page.getByRole("link", { name: "Categorias" }).first()).toBeVisible();
  24 | 
  25 |     await page.getByRole("link", { name: "Relatórios" }).first().click();
  26 |     await expect(page).toHaveURL(/\/totais$/);
  27 |     await expect(page.getByRole("heading", { name: "Totais por Pessoa" })).toBeVisible();
  28 |   });
  29 | 
  30 |   test("deve manter o shell visivel quando a API de transacoes falha", async ({ page }) => {
  31 |     await page.route("**/api/v1/Transacoes**", async route => {
  32 |       await route.fulfill({
  33 |         status: 500,
  34 |         contentType: "application/json",
  35 |         body: JSON.stringify({ title: "Erro interno simulado" })
  36 |       });
  37 |     });
  38 | 
  39 |     await page.goto("/transacoes");
  40 | 
  41 |     await expect(page.getByText("Minhas Finanças")).toBeVisible();
  42 |     await expect(page.getByRole("link", { name: "Transações" }).first()).toBeVisible();
  43 |     await expect(page.locator("main")).toContainText("Carregando...");
  44 |   });
  45 | 
  46 |   test("deve nao quebrar o chrome da aplicacao em rota inexistente", async ({ page }) => {
  47 |     await page.goto("/rota-inexistente");
  48 | 
> 49 |     await expect(page.getByText("Minhas Finanças")).toBeVisible();
     |                                                     ^ Error: expect(locator).toBeVisible() failed
  50 |     await expect(page.getByRole("link", { name: "Dashboard" }).first()).toBeVisible();
  51 |     await expect(page).toHaveURL(/\/rota-inexistente$/);
  52 |   });
  53 | });
  54 | 
```