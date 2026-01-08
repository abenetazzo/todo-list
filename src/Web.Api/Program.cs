using Microsoft.EntityFrameworkCore;
using Web.Api.Services;
using Web.Api.Data;
using Web.Api.Models;

namespace Web.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        // DbContext condizionale
        if (builder.Environment.IsEnvironment("Testing"))
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("IntegrationTestDb"));
        }
        else
        {
            var conn = builder.Configuration.GetConnectionString("DefaultConnection")!;
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(conn));
        }
        builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Debug);
        builder.Services.AddScoped<ITodoService, TodoService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        // Endpoints
        app.MapGet("/todos", async (ITodoService service) =>
            await service.GetAllAsync())
            .WithName("GetTodos");
        app.MapGet("/todos/{id}", async (int id, ITodoService service) =>
            await service.GetByIdAsync(id) is TodoItem todo ? Results.Ok(todo) : Results.NotFound())
            .WithName("GetTodoById");
        app.MapPost("/todos", async (CreateTodoItemDTO newTodo, ITodoService service) =>
            await service.CreateAsync(newTodo) is TodoItem todoItem ? Results.Created($"/todos/{todoItem.Id}", todoItem) : Results.BadRequest())
            .WithName("AddTodo");
        app.MapDelete("/todos/{id}", async (int id, ITodoService service) =>
            await service.DeleteAsync(id) ? Results.NoContent() : Results.NotFound())
            .WithName("DeleteTodo");
        app.MapPut("/todos/{id}", async (int id, UpdateTodoItemDTO updatedTodo, ITodoService service) =>
            await service.UpdateAsync(id, updatedTodo) is TodoItem todo ? Results.Ok(todo) : Results.NotFound())
            .WithName("UpdateTodo");
        app.MapPatch("/todos/{id}", async (int id, PatchTodoItemDTO patchedTodo, ITodoService service) =>
            await service.PatchAsync(id, patchedTodo) is TodoItem todo ? Results.Ok(todo) : Results.NotFound())
            .WithName("PatchTodo");

        // Run the application
        app.Run();
    }
}