using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        var count = await _todoService.SyncAsync();

        return Ok(new
        {
            message = "Sincronização concluída com sucesso.",
            count
        });
    }

    [HttpGet("todos")]
    public async Task<IActionResult> GetTodos(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? title = null,
        [FromQuery] string sort = "id",
        [FromQuery] string order = "asc")
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(new
            {
                message = "Page e pageSize devem ser maiores que zero."
            });
        }

        var (todos, total) = await _todoService.GetTodosAsync(
            page,
            pageSize,
            title,
            sort,
            order
        );

        return Ok(new
        {
            page,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling(total / (double)pageSize),
            data = todos
        });
    }

    [HttpGet("todos/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var todo = await _todoService.GetByIdAsync(id);

        if (todo is null)
        {
            return NotFound(new
            {
                message = $"Tarefa com ID {id} não encontrada."
            });
        }

        return Ok(todo);
    }

    [HttpPut("todos/{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateTodoDto dto)
    {
        var existingTodo = await _todoService.GetByIdAsync(id);

        if (existingTodo is null)
        {
            return NotFound(new
            {
                message = $"Tarefa com ID {id} não encontrada."
            });
        }

        var (todo, error) =
            await _todoService.UpdateCompletedAsync(id, dto.Completed);

        if (error is not null)
        {
            return BadRequest(new
            {
                message = error
            });
        }

        return Ok(todo);
    }
}