using Web.Api.Models;

namespace Web.Api.Services;

public interface ITodoService
{
    Task<List<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(int id);
    Task<TodoItem> CreateAsync(CreateTodoItemDTO dto);
    Task<bool> DeleteAsync(int id);
    Task<TodoItem?> UpdateAsync(int id, UpdateTodoItemDTO dto);
    Task<TodoItem?> PatchAsync(int id, PatchTodoItemDTO dto);
}