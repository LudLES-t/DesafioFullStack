using System.Net;
using System.Net.Http.Json;

namespace TodoApi.Tests;

public class TodosIntegrationTests :
    IClassFixture<TodoApiFactory>
{
    private readonly HttpClient _client;

    public TodosIntegrationTests(TodoApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTodos_DeveAplicarPaginacao()
    {
        var response = await _client.GetAsync(
            "/todos?page=1&pageSize=2"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("\"pageSize\":2", content);
    }

    [Fact]
    public async Task GetTodos_DeveFiltrarPorTitulo()
    {
        var response = await _client.GetAsync(
            "/todos?title=expedita"
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Expedita exemplo", content);
    }

    [Fact]
    public async Task GetTodo_DeveRetornarTarefaPorId()
    {
        var response = await _client.GetAsync("/todos/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Put_DeveImpedirMaisDeCincoIncompletas()
    {
        var response = await _client.PutAsJsonAsync(
            "/todos/6",
            new { completed = false }
        );

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode
        );
    }

    [Fact]
    public async Task Put_DeveAtualizarCompleted()
    {
        var response = await _client.PutAsJsonAsync(
            "/todos/1",
            new { completed = true }
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}