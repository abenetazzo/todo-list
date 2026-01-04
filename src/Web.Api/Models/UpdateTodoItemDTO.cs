namespace Web.Api.Models;

public class UpdateTodoItemDTO
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}