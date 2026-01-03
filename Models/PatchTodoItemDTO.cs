namespace WebApi.Models;

public class PatchTodoItemDTO
{
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
}