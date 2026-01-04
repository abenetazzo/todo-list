using Web.Api.Models;
using Web.Api.Services;

namespace Web.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddScoped<TodoService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        // Endpoints
        app.MapGet("/todos", (TodoService service) => service.GetAll())
            .WithName("GetTodos");
        app.MapGet("/todos/{id}", (int id, TodoService service) => service.GetById(id) is TodoItem todo ? Results.Ok(todo) : Results.NotFound())
            .WithName("GetTodoById");
        app.MapPost("/todos", (CreateTodoItemDTO newTodo, TodoService service) => service.Create(newTodo) is TodoItem todoItem ? Results.Created($"/todos/{todoItem.Id}", todoItem) : Results.BadRequest())
            .WithName("AddTodo");
        app.MapDelete("/todos/{id}", (int id, TodoService service) => service.Delete(id) ? Results.NoContent() : Results.NotFound())
            .WithName("DeleteTodo");
        app.MapPut("/todos/{id}", (int id, UpdateTodoItemDTO updatedTodo, TodoService service) => service.Update(id, updatedTodo) is TodoItem todo ? Results.Ok(todo) : Results.NotFound())
            .WithName("UpdateTodo");
        app.MapPatch("/todos/{id}", (int id, PatchTodoItemDTO patchedTodo, TodoService service) => service.Patch(id, patchedTodo) is TodoItem todo ? Results.Ok(todo) : Results.NotFound())
            .WithName("PatchTodo");

        // Run the application
        app.Run();
    }
}