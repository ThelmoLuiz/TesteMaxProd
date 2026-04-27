using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;

namespace MinhasFinancas.IntegrationTests;

[Collection("Api serial")]
public class SwaggerApiCoverageTests
{
    private static readonly Uri BaseUri = new(
        Environment.GetEnvironmentVariable("MINHASFINANCAS_API_BASE_URL") ?? "http://localhost:5000",
        UriKind.Absolute);

    [Fact]
    public async Task SwaggerJson_DeveEstarDisponivel()
    {
        using var http = CreateClient();

        using var response = await http.GetAsync("/swagger/v1/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertJsonResponseAsync(response);
    }

    [Fact]
    public async Task GetCategorias_DeveRetornarJson()
    {
        using var http = CreateClient();

        using var response = await http.GetAsync("/api/v1/Categorias");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertJsonResponseAsync(response);
    }

    [Fact]
    public async Task PostCategorias_DeveCriarCategoria()
    {
        using var http = CreateClient();

        using var response = await http.PostAsync(
            "/api/v1/Categorias",
            JsonContent(new
            {
                descricao = NomeUnico("Categoria teste"),
                finalidade = 1
            }));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await AssertJsonResponseAsync(response);
        json.GetProperty("id").GetGuid().Should().NotBeEmpty();
    }

    [Fact]
    public async Task PostCategorias_DeveValidarFalhaQuandoPayloadInvalido()
    {
        using var http = CreateClient();

        using var response = await http.PostAsync(
            "/api/v1/Categorias",
            JsonContent(new { }));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCategoriaPorId_DeveRetornarCategoriaCriada()
    {
        using var http = CreateClient();
        var categoriaId = await CriarCategoriaAsync(http);

        using var response = await http.GetAsync($"/api/v1/Categorias/{categoriaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await AssertJsonResponseAsync(response);
        json.GetProperty("id").GetGuid().Should().Be(categoriaId);
    }

    [Fact]
    public async Task GetCategoriaPorId_DeveRetornarNotFoundQuandoIdNaoExiste()
    {
        using var http = CreateClient();

        using var response = await http.GetAsync($"/api/v1/Categorias/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPessoas_DeveRetornarJson()
    {
        using var http = CreateClient();

        using var response = await http.GetAsync("/api/v1/Pessoas");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertJsonResponseAsync(response);
    }

    [Fact]
    public async Task PostPessoas_DeveCriarPessoa()
    {
        using var http = CreateClient();

        using var response = await http.PostAsync(
            "/api/v1/Pessoas",
            JsonContent(new
            {
                nome = NomeUnico("Pessoa teste"),
                dataNascimento = "1990-05-20"
            }));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await AssertJsonResponseAsync(response);
        json.GetProperty("id").GetGuid().Should().NotBeEmpty();
    }

    [Fact]
    public async Task PostPessoas_DeveValidarFalhaQuandoPayloadInvalido()
    {
        using var http = CreateClient();

        using var response = await http.PostAsync(
            "/api/v1/Pessoas",
            JsonContent(new { }));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPessoaPorId_DeveRetornarPessoaCriada()
    {
        using var http = CreateClient();
        var pessoaId = await CriarPessoaAsync(http);

        using var response = await http.GetAsync($"/api/v1/Pessoas/{pessoaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await AssertJsonResponseAsync(response);
        json.GetProperty("id").GetGuid().Should().Be(pessoaId);
    }

    [Fact]
    public async Task GetPessoaPorId_DeveRetornarNotFoundQuandoIdNaoExiste()
    {
        using var http = CreateClient();

        using var response = await http.GetAsync($"/api/v1/Pessoas/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PutPessoa_DeveAtualizarPessoaExistente()
    {
        using var http = CreateClient();
        var pessoaId = await CriarPessoaAsync(http);
        var novoNome = NomeUnico("Pessoa atualizada");

        using var updateResponse = await http.PutAsync(
            $"/api/v1/Pessoas/{pessoaId}",
            JsonContent(new
            {
                nome = novoNome,
                dataNascimento = "1991-06-21"
            }));

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var getResponse = await http.GetAsync($"/api/v1/Pessoas/{pessoaId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await AssertJsonResponseAsync(getResponse);
        json.GetProperty("nome").GetString().Should().Be(novoNome);
    }

    [Fact]
    public async Task PutPessoa_DeveValidarFalhaQuandoPayloadInvalido()
    {
        using var http = CreateClient();
        var pessoaId = await CriarPessoaAsync(http);

        using var response = await http.PutAsync(
            $"/api/v1/Pessoas/{pessoaId}",
            JsonContent(new { }));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeletePessoa_DeveRemoverPessoaExistente()
    {
        using var http = CreateClient();
        var pessoaId = await CriarPessoaAsync(http);

        using var deleteResponse = await http.DeleteAsync($"/api/v1/Pessoas/{pessoaId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var getResponse = await http.GetAsync($"/api/v1/Pessoas/{pessoaId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePessoa_DeveRetornarNotFoundQuandoIdNaoExiste()
    {
        using var http = CreateClient();

        using var response = await http.DeleteAsync($"/api/v1/Pessoas/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTotaisPorPessoa_DeveRetornarJson()
    {
        using var http = CreateClient();

        using var response = await http.GetAsync("/api/v1/Totais/pessoas");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertJsonResponseAsync(response);
    }

    [Fact]
    public async Task GetTotaisPorCategoria_DeveRetornarJson()
    {
        using var http = CreateClient();

        using var response = await http.GetAsync("/api/v1/Totais/categorias");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertJsonResponseAsync(response);
    }

    [Fact]
    public async Task GetTransacoes_DeveRetornarJson()
    {
        using var http = CreateClient();

        using var response = await http.GetAsync("/api/v1/Transacoes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await AssertJsonResponseAsync(response);
    }

    [Fact]
    public async Task PostTransacoes_DeveCriarTransacao()
    {
        using var http = CreateClient();
        var categoriaId = await CriarCategoriaAsync(http);
        var pessoaId = await CriarPessoaAsync(http);

        using var response = await http.PostAsync(
            "/api/v1/Transacoes",
            JsonContent(new
            {
                descricao = NomeUnico("Transacao teste"),
                valor = 150.35,
                tipo = 0,
                categoriaId,
                pessoaId,
                data = DateTime.UtcNow.ToString("O")
            }));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await AssertJsonResponseAsync(response);
        json.GetProperty("id").GetGuid().Should().NotBeEmpty();
    }

    [Fact]
    public async Task PostTransacoes_DeveValidarFalhaQuandoPayloadInvalido()
    {
        using var http = CreateClient();

        using var response = await http.PostAsync(
            "/api/v1/Transacoes",
            JsonContent(new
            {
                descricao = "",
                valor = 0,
                tipo = 0,
                categoriaId = Guid.Empty,
                pessoaId = Guid.Empty,
                data = DateTime.UtcNow.ToString("O")
            }));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTransacaoPorId_DeveRetornarTransacaoCriada()
    {
        using var http = CreateClient();
        var categoriaId = await CriarCategoriaAsync(http);
        var pessoaId = await CriarPessoaAsync(http);
        var transacaoId = await CriarTransacaoAsync(http, categoriaId, pessoaId);

        using var response = await http.GetAsync($"/api/v1/Transacoes/{transacaoId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await AssertJsonResponseAsync(response);
        json.GetProperty("id").GetGuid().Should().Be(transacaoId);
    }

    [Fact]
    public async Task GetTransacaoPorId_DeveRetornarNotFoundQuandoIdNaoExiste()
    {
        using var http = CreateClient();

        using var response = await http.GetAsync($"/api/v1/Transacoes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    private static HttpClient CreateClient() => new() { BaseAddress = BaseUri };

    private static StringContent JsonContent(object value) =>
        new(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");

    private static string NomeUnico(string prefixo) => $"{prefixo} {DateTime.UtcNow:yyyyMMddHHmmssfff}";

    private static async Task<Guid> CriarCategoriaAsync(HttpClient http)
    {
        using var response = await http.PostAsync(
            "/api/v1/Categorias",
            JsonContent(new
            {
                descricao = NomeUnico("Categoria dependente"),
                finalidade = 1
            }));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await AssertJsonResponseAsync(response);
        return json.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CriarPessoaAsync(HttpClient http)
    {
        using var response = await http.PostAsync(
            "/api/v1/Pessoas",
            JsonContent(new
            {
                nome = NomeUnico("Pessoa dependente"),
                dataNascimento = "1990-01-10"
            }));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await AssertJsonResponseAsync(response);
        return json.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CriarTransacaoAsync(HttpClient http, Guid categoriaId, Guid pessoaId)
    {
        using var response = await http.PostAsync(
            "/api/v1/Transacoes",
            JsonContent(new
            {
                descricao = NomeUnico("Transacao dependente"),
                valor = 49.99,
                tipo = 0,
                categoriaId,
                pessoaId,
                data = DateTime.UtcNow.ToString("O")
            }));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await AssertJsonResponseAsync(response);
        return json.GetProperty("id").GetGuid();
    }

    private static async Task<JsonElement> AssertJsonResponseAsync(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadAsStringAsync();
        payload.Should().NotBeNullOrWhiteSpace();

        using var document = JsonDocument.Parse(payload);
        document.RootElement.ValueKind.Should().NotBe(JsonValueKind.Undefined);
        return document.RootElement.Clone();
    }

    private static async Task AssertProblemDetailsAsync(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
    {
        response.StatusCode.Should().Be(expectedStatusCode);
        var json = await AssertJsonResponseAsync(response);
        json.TryGetProperty("title", out _).Should().BeTrue();
    }
}

[CollectionDefinition("Api serial", DisableParallelization = true)]
public class ApiSerialCollectionDefinition
{
}
