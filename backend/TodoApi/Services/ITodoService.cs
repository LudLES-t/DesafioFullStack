using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    Task<int> SyncAsync();

    Task<(List<Todo> Todos, int Total)> GetTodosAsync(
        int page,
        int pageSize,
        string? title,
        string sort,
        string order
    );

    Task<Todo?> GetByIdAsync(int id);

    Task<(Todo? Todo, string? Error)> UpdateCompletedAsync(
        int id,
        bool completed
    );
}