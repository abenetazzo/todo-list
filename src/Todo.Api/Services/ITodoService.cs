using Todo.Domain.Todos;

namespace Todo.Api.Services;

public interface ITodoService
{
    Task<List<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(Guid id);
    Task<TodoItem> CreateAsync(CreateTodoItemDTO dto);
    Task<bool> DeleteAsync(Guid id);
    Task<TodoItem?> UpdateAsync(Guid id, UpdateTodoItemDTO dto);
    Task<TodoItem?> PatchAsync(Guid id, PatchTodoItemDTO dto);
}