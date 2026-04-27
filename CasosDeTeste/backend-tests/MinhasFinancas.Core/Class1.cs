namespace MinhasFinancas.Core;

public enum TipoTransacao
{
	Receita = 1,
	Despesa = 2
}

public sealed record Transacao(decimal Valor, TipoTransacao Tipo)
{
	public static Transacao Criar(decimal valor, TipoTransacao tipo)
	{
		if (valor <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(valor), "O valor deve ser maior que zero.");
		}

		return new Transacao(valor, tipo);
	}
}

public sealed class ResumoFinanceiroService
{
	public decimal CalcularSaldo(IEnumerable<Transacao> transacoes)
	{
		ArgumentNullException.ThrowIfNull(transacoes);

		return transacoes.Sum(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);
	}

	public (decimal Receitas, decimal Despesas) CalcularTotais(IEnumerable<Transacao> transacoes)
	{
		ArgumentNullException.ThrowIfNull(transacoes);

		var receitas = transacoes
			.Where(t => t.Tipo == TipoTransacao.Receita)
			.Sum(t => t.Valor);

		var despesas = transacoes
			.Where(t => t.Tipo == TipoTransacao.Despesa)
			.Sum(t => t.Valor);

		return (receitas, despesas);
	}
}
