using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService : ITodoService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public TodoService(
        AppDbContext context,
        HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task<int> SyncAsync()
    {
        var todos = await _httpClient.GetFromJsonAsync<List<Todo>>(
            "https://jsonplaceholder.typicode.com/todos"
        );

        if (todos is null)
        {
            return 0;
        }

        foreach (var todo in todos)
        {
            var exists = await _context.Todos
                .AnyAsync(t => t.Id == todo.Id);

            if (!exists)
            {
                _context.Todos.Add(todo);
            }
        }

        await _context.SaveChangesAsync();

        return todos.Count;
    }

    public async Task<(List<Todo> Todos, int Total)> GetTodosAsync(
        int page,
        int pageSize,
        string? title,
        string sort,
        string order)
    {
        var query = _context.Todos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(todo =>
                todo.Title.ToLower().Contains(title.ToLower()));
        }

        query = sort.ToLower() switch
        {
            "title" => order.ToLower() == "desc"
                ? query.OrderByDescending(todo => todo.Title)
                : query.OrderBy(todo => todo.Title),

            "userid" => order.ToLower() == "desc"
                ? query.OrderByDescending(todo => todo.UserId)
                : query.OrderBy(todo => todo.UserId),

            _ => order.ToLower() == "desc"
                ? query.OrderByDescending(todo => todo.Id)
                : query.OrderBy(todo => todo.Id)
        };

        var total = await query.CountAsync();

        var todos = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (todos, total);
        
    }
    public async Task<Todo?> GetByIdAsync(int id)
{
    return await _context.Todos
        .AsNoTracking()
        .FirstOrDefaultAsync(todo => todo.Id == id);
}

public async Task<(Todo? Todo, string? Error)> UpdateCompletedAsync(
    int id,
    bool completed)
{
    var todo = await _context.Todos
        .FirstOrDefaultAsync(todo => todo.Id == id);

    if (todo is null)
    {
        return (null, "Tarefa não encontrada.");
    }

    // A regra só precisa ser verificada quando uma tarefa
    // completa está sendo transformada em incompleta.
    if (todo.Completed && !completed)
    {
        var incompleteCount = await _context.Todos
            .CountAsync(t =>
                t.UserId == todo.UserId &&
                !t.Completed &&
                t.Id != todo.Id);

        if (incompleteCount >= 5)
        {
            return (
                null,
                $"O usuário {todo.UserId} já possui 5 tarefas incompletas."
            );
        }
    }

    todo.Completed = completed;

    await _context.SaveChangesAsync();

    return (todo, null);
}
}