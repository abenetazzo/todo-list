namespace Todo.Domain.Todos;

public class PatchTodoItemDTO
{
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
}