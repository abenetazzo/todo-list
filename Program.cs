using WebApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// step 1: Basic Ping Endpoint

app.MapGet("/ping", () => "pong")
   .WithName("Ping");

// step 2: Todo List Endpoint

List<TodoItem> todos =
[
    new TodoItem { Id = 1, Title = "Learn ASP.NET Core", IsCompleted = false },
    new TodoItem { Id = 2, Title = "Build Web API", IsCompleted = false },
    new TodoItem { Id = 3, Title = "Write Documentation", IsCompleted = true }
];

app.MapGet("/todos", () => todos )
   .WithName("GetTodos");

// step 3: Add Todo Item Endpoint

app.MapPost("/todos", (CreateTodoItemDTO newTodo) =>
{
    var todoItem = new TodoItem
    {
        Id = todos.Any() ? todos.Max(t => t.Id) + 1 : 1,
        Title = newTodo.Title,
        IsCompleted = newTodo.IsCompleted
    };
    todos.Add(todoItem);
    return Results.Created($"/todos/{todoItem.Id}", todoItem);
}).WithName("AddTodo");

// step 4: Get Todo Item by ID Endpoint & Delete Todo Item Endpoint

app.MapGet("/todos/{id}", (int id) =>
{
    var todo = todos.SingleOrDefault(t => t.Id == id);
    return todo switch
    {
        not null => Results.Ok(todo),
        null => Results.NotFound()
    };
}).WithName("GetTodoById");

app.MapDelete("/todos/{id}", (int id) =>
{
    var todo = todos.SingleOrDefault(t => t.Id == id);
    if (todo is not null)
    {
        todos.Remove(todo);
        return Results.NoContent();
    }
    return Results.NotFound();
}).WithName("DeleteTodo");

// step 5: Update Todo Item Endpoint & Patch Todo Item Endpoint

app.MapPut("/todos/{id}", (int id, UpdateTodoItemDTO updatedTodo) =>
{
    var todo = todos.SingleOrDefault(t => t.Id == id);
    if (todo is not null)
    {
        todo.Title = updatedTodo.Title;
        todo.IsCompleted = updatedTodo.IsCompleted;
        return Results.Ok(todo);
    }
    return Results.NotFound();
}).WithName("UpdateTodo");

app.MapPatch("/todos/{id}", (int id, PatchTodoItemDTO patchedTodo) =>
{
    var todo = todos.SingleOrDefault(t => t.Id == id);
    if (todo is not null)
    {
        if (patchedTodo.Title is not null)
        {
            todo.Title = patchedTodo.Title;
        }
        if (patchedTodo.IsCompleted.HasValue)
        {
            todo.IsCompleted = patchedTodo.IsCompleted.Value;
        }
        return Results.Ok(todo);
    }
    return Results.NotFound();
}).WithName("PatchTodo");

app.Run();