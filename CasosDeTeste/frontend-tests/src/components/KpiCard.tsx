import type { ReactNode } from "react";

export type KpiCardProps = {
  title: string;
  value: number;
  icon?: ReactNode;
};

export function formatCurrencyBRL(value: number): string {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
    minimumFractionDigits: 2
  }).format(value);
}

export function KpiCard({ title, value, icon }: KpiCardProps) {
  return (
    <section aria-label={`KPI ${title}`}>
      <div>{icon}</div>
      <h2>{title}</h2>
      <strong>{formatCurrencyBRL(value)}</strong>
    </section>
  );
}
