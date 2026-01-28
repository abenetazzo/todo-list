namespace Web.Domain.Todos;

public class UpdateTodoItemDTO
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}