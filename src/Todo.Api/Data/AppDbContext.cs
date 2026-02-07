using Microsoft.EntityFrameworkCore;
using Todo.Domain.Todos;

namespace Todo.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;
}