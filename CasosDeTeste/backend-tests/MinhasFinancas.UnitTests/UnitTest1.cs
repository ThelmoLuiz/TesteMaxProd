using FluentAssertions;
using MinhasFinancas.Core;

namespace MinhasFinancas.UnitTests;

public class ResumoFinanceiroServiceTests
{
    [Fact]
    public void CalcularSaldo_DeveSubtrairDespesasDasReceitas()
    {
        var service = new ResumoFinanceiroService();
        var transacoes = new[]
        {
            Transacao.Criar(1200m, TipoTransacao.Receita),
            Transacao.Criar(300m, TipoTransacao.Despesa),
            Transacao.Criar(150m, TipoTransacao.Despesa)
        };

        var saldo = service.CalcularSaldo(transacoes);

        saldo.Should().Be(750m);
    }

    [Fact]
    public void CalcularTotais_DeveSepararReceitasEDespesas()
    {
        var service = new ResumoFinanceiroService();
        var transacoes = new[]
        {
            Transacao.Criar(100m, TipoTransacao.Receita),
            Transacao.Criar(50m, TipoTransacao.Despesa),
            Transacao.Criar(80m, TipoTransacao.Receita)
        };

        var totais = service.CalcularTotais(transacoes);

        totais.Receitas.Should().Be(180m);
        totais.Despesas.Should().Be(50m);
    }

    [Fact]
    public void CriarTransacao_DeveLancarExcecaoQuandoValorInvalido()
    {
        var acao = () => Transacao.Criar(0m, TipoTransacao.Despesa);

        acao.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CalcularSaldo_DeveRetornarZeroQuandoNaoHouverTransacoes()
    {
        var service = new ResumoFinanceiroService();

        var saldo = service.CalcularSaldo(Array.Empty<Transacao>());

        saldo.Should().Be(0m);
    }

    [Fact]
    public void CalcularSaldo_DeveLancarExcecaoQuandoColecaoForNula()
    {
        var service = new ResumoFinanceiroService();

        var acao = () => service.CalcularSaldo(null!);

        acao.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CalcularTotais_DeveLancarExcecaoQuandoColecaoForNula()
    {
        var service = new ResumoFinanceiroService();

        var acao = () => service.CalcularTotais(null!);

        acao.Should().Throw<ArgumentNullException>();
    }
}
