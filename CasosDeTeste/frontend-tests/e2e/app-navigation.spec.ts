import { expect, test } from "@playwright/test";

test.describe("Navegacao principal", () => {
  test("deve abrir dashboard e exibir shell da aplicacao", async ({ page }) => {
    await page.goto("/");

    await expect(page.getByText("Minhas Finanças")).toBeVisible();
    await expect(page.getByRole("link", { name: "Dashboard" }).first()).toBeVisible();
    await expect(page.getByRole("link", { name: "Transações" }).first()).toBeVisible();
    await expect(page.getByRole("link", { name: "Categorias" }).first()).toBeVisible();
    await expect(page.getByRole("link", { name: "Relatórios" }).first()).toBeVisible();
  });

  test("deve navegar para paginas principais", async ({ page }) => {
    await page.goto("/");

    await page.getByRole("link", { name: "Transações" }).first().click();
    await expect(page).toHaveURL(/\/transacoes$/);
    await expect(page.getByRole("link", { name: "Transacoes" })).toBeVisible();

    await page.getByRole("link", { name: "Categorias" }).first().click();
    await expect(page).toHaveURL(/\/categorias$/);
    await expect(page.getByRole("link", { name: "Categorias" }).first()).toBeVisible();

    await page.getByRole("link", { name: "Relatórios" }).first().click();
    await expect(page).toHaveURL(/\/totais$/);
    await expect(page.getByRole("heading", { name: "Totais por Pessoa" })).toBeVisible();
  });

  test("deve manter o shell visivel quando a API de transacoes falha", async ({ page }) => {
    await page.route("**/api/v1/Transacoes**", async route => {
      await route.fulfill({
        status: 500,
        contentType: "application/json",
        body: JSON.stringify({ title: "Erro interno simulado" })
      });
    });

    await page.goto("/transacoes");

    await expect(page.getByText("Minhas Finanças")).toBeVisible();
    await expect(page.getByRole("link", { name: "Transações" }).first()).toBeVisible();
    await expect(page.locator("main")).toContainText("Carregando...");
  });

  test("deve nao quebrar o chrome da aplicacao em rota inexistente", async ({ page }) => {
    test.skip(true, "Pendente: falta fallback de rota para manter o shell em not found.");

    await page.goto("/rota-inexistente");

    await expect(page.getByText("Minhas Finanças")).toBeVisible();
    await expect(page.getByRole("link", { name: "Dashboard" }).first()).toBeVisible();
    await expect(page).toHaveURL(/\/rota-inexistente$/);
  });
});
