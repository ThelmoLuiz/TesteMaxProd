import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { KpiCard, formatCurrencyBRL } from "./KpiCard";

describe("formatCurrencyBRL", () => {
  it("deve formatar numero em BRL", () => {
    expect(formatCurrencyBRL(1234.56)).toBe("R$\u00a01.234,56");
  });

  it("deve formatar valor negativo em BRL", () => {
    expect(formatCurrencyBRL(-1234.56)).toBe("-R$\u00a01.234,56");
  });
});

describe("KpiCard", () => {
  it("deve renderizar titulo e valor formatado", () => {
    render(<KpiCard title="Saldo Atual" value={2500.4} icon={<span>$</span>} />);

    expect(screen.getByRole("heading", { name: "Saldo Atual" })).toBeInTheDocument();
    expect(screen.getByText(/R\$\s*2\.500,40/)).toBeInTheDocument();
    expect(screen.getByLabelText("KPI Saldo Atual")).toBeInTheDocument();
  });

  it("deve renderizar sem icone sem quebrar a acessibilidade", () => {
    render(<KpiCard title="Despesas" value={-50} />);

    expect(screen.getByRole("heading", { name: "Despesas" })).toBeInTheDocument();
    expect(screen.getByLabelText("KPI Despesas")).toBeInTheDocument();
    expect(screen.getByText(/-R\$\s*50,00/)).toBeInTheDocument();
  });
});
