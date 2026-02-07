namespace Todo.Domain.Todos;

public class CreateTodoItemDTO
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}