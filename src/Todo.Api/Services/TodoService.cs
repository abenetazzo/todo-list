using Microsoft.EntityFrameworkCore;
using Todo.Api.Data;
using Todo.Domain.Todos;

namespace Todo.Api.Services;

public class TodoService: ITodoService
{
    private readonly AppDbContext _context;

    public TodoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TodoItem>> GetAllAsync() =>
        await _context.TodoItems.ToListAsync();

    public async Task<TodoItem?> GetByIdAsync(Guid id) =>
        await _context.TodoItems.FindAsync(id);

    public async Task<TodoItem> CreateAsync(CreateTodoItemDTO dto)
    {
        var newTodo = new TodoItem
        {
            Title = dto.Title,
            IsCompleted = dto.IsCompleted
        };
        _context.TodoItems.Add(newTodo);
        await _context.SaveChangesAsync();
        return newTodo;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var todo = await GetByIdAsync(id);
        if (todo is null) return false;
        _context.TodoItems.Remove(todo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TodoItem?> UpdateAsync(Guid id, UpdateTodoItemDTO dto)
    {
        var todo = await GetByIdAsync(id);
        if (todo is null) return null;
        todo.Title = dto.Title;
        todo.IsCompleted = dto.IsCompleted;
        await _context.SaveChangesAsync();
        return todo;
    }

    public async Task<TodoItem?> PatchAsync(Guid id, PatchTodoItemDTO dto)
    {
        var todo = await GetByIdAsync(id);
        if (todo is null) return null;
        if (dto.Title is not null) todo.Title = dto.Title;
        if (dto.IsCompleted.HasValue) todo.IsCompleted = dto.IsCompleted.Value;
        await _context.SaveChangesAsync();
        return todo;
    }
}