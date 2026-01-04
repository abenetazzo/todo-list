using System;
using Web.Api.Models;

namespace Web.Api.Services;

public class TodoService
{
    private readonly List<TodoItem> _todos = new()
    {
        new TodoItem { Id = 1, Title = "Learn ASP.NET Core", IsCompleted = false },
        new TodoItem { Id = 2, Title = "Build a Web API", IsCompleted = false },
        new TodoItem { Id = 3, Title = "Write Documentation", IsCompleted = true }
    };

    public List<TodoItem> GetAll() => _todos;

    public TodoItem? GetById(int id) => _todos.SingleOrDefault(t => t.Id == id);

    public TodoItem Create(CreateTodoItemDTO dto)
    {
        var newTodo = new TodoItem
        {
            Id = _todos.Any() ? _todos.Max(t => t.Id) + 1 : 1,
            Title = dto.Title,
            IsCompleted = dto.IsCompleted
        };
        _todos.Add(newTodo);
        return newTodo;
    }

    public bool Delete(int id)
    {
        var todo = GetById(id);
        if (todo is null) return false;
        _todos.Remove(todo);
        return true;
    }

    public TodoItem? Update(int id, UpdateTodoItemDTO dto)
    {
        var todo = GetById(id);
        if (todo is null) return null;
        todo.Title = dto.Title;
        todo.IsCompleted = dto.IsCompleted;
        return todo;
    }

    public TodoItem? Patch(int id, PatchTodoItemDTO dto)
    {
        var todo = GetById(id);
        if (todo is null) return null;
        if (dto.Title is not null) todo.Title = dto.Title;
        if (dto.IsCompleted.HasValue) todo.IsCompleted = dto.IsCompleted.Value;
        return todo;
    }
}