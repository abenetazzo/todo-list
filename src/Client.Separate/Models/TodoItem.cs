namespace Client.Separate.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}

public class CreateTodoItemDTO
{
    public string Title { get; set; } = string.Empty;
}

public class UpdateTodoItemDTO
{
    public string Title { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}