namespace Web.Api.Models;

public class CreateTodoItemDTO
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}