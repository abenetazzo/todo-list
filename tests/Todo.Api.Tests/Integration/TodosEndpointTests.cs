using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Todo.Domain.Todos;
using Todo.Api.Data;
using Todo.Api.Services;

namespace Todo.Api.Tests.Integration;

public class TodosEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TodosEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task SetDatabase()
    {
        using var scope = new CustomWebApplicationFactory().Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Assicurati che il database sia creato
        await context.Database.EnsureCreatedAsync();

        // Pulisci il database prima di ogni test
        context.TodoItems.RemoveRange(context.TodoItems);
        await context.SaveChangesAsync();

        // Seed dei dati iniziali
        context.TodoItems.AddRange(
            new TodoItem { Id = new Guid("2d41d56a-ac21-475d-8fbc-62d54f10ce94"), Title = "Primo todoItem", IsCompleted = false },
            new TodoItem { Id = new Guid("ca781d57-5ec2-45fd-8726-4157286f4889"), Title = "Secondo todoItem", IsCompleted = false },
            new TodoItem { Id = new Guid("3ddcf8cc-dabb-4492-ab41-e81d54fd05c2"), Title = "Test finale ok", IsCompleted = true }
        );
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetTodos_ReturnsInitialList()
    {
        // Arrange
        await SetDatabase();

        // Act
        var response = await _client.GetAsync("/todos");
        var todos = await response.Content.ReadFromJsonAsync<List<TodoItem>>();

    // Debug: stampa il contenuto di todos
    Console.WriteLine("Contenuto di todos:");
    if (todos != null)
    {
        foreach (var todo in todos)
        {
            Console.WriteLine($"Id: {todo.Id}, Title: {todo.Title}, IsCompleted: {todo.IsCompleted}");
        }
    }
    else
    {
        Console.WriteLine("todos è null");
    }

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        todos.Should().NotBeNull();
        todos!.Count.Should().Be(3);
    }
    
    [Fact]
    public async Task GetTodoById_ReturnsCorrectTodo()
    {
        // Arrange
        await SetDatabase();
        var expectedTodo = new TodoItem { Id = new Guid("2d41d56a-ac21-475d-8fbc-62d54f10ce94"), Title = "Primo todoItem", IsCompleted = false };

        // Act
        var response = await _client.GetAsync("/todos/2d41d56a-ac21-475d-8fbc-62d54f10ce94");
        var todo = await response.Content.ReadFromJsonAsync<TodoItem>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(expectedTodo.Id);
        todo.Title.Should().Be(expectedTodo.Title);
        todo.IsCompleted.Should().Be(expectedTodo.IsCompleted);
    }

    [Fact]
    public async Task GetTodoById_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        await SetDatabase();

        // Act
        var response = await _client.GetAsync("/todos/f3c1a4c2-9e7b-4c8e-9d7a-2b4f6e8c1a55");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PostTodo_CreatesNewTodo()
    {
        // Arrange
        await SetDatabase();
        var newTodo = new TodoItem { Title = "Write Integration Tests", IsCompleted = false };

        // Act
        var response = await _client.PostAsJsonAsync("/todos", newTodo);
        var createdTodo = await response.Content.ReadFromJsonAsync<TodoItem>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdTodo.Should().NotBeNull();
        createdTodo.Title.Should().Be(newTodo.Title);
        createdTodo.IsCompleted.Should().Be(newTodo.IsCompleted);
    }

    [Fact]
    public async Task DeleteTodo_ExistingId_ReturnsNoContent()
    {
        // Arrange
        await SetDatabase();

        // Act
        var response = await _client.DeleteAsync("/todos/3ddcf8cc-dabb-4492-ab41-e81d54fd05c2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTodo_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        await SetDatabase();

        // Act
        var response = await _client.DeleteAsync("/todos/f3c1a4c2-9e7b-4c8e-9d7a-2b4f6e8c1a55");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PutTodo_UpdatesExistingTodo()
    {
        // Arrange
        await SetDatabase();
        var updatedTodo = new TodoItem { Id = new Guid("ca781d57-5ec2-45fd-8726-4157286f4889"), Title = "Learn ASP.NET Core - Updated", IsCompleted = true };

        // Act
        var response = await _client.PutAsJsonAsync("/todos/ca781d57-5ec2-45fd-8726-4157286f4889", updatedTodo);
        var todo = await response.Content.ReadFromJsonAsync<TodoItem>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(updatedTodo.Id);
        todo.Title.Should().Be(updatedTodo.Title);
        todo.IsCompleted.Should().Be(updatedTodo.IsCompleted);
    }

    [Fact]
    public async Task PutTodo_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        await SetDatabase();
        var updatedTodo = new TodoItem { Id = new Guid(), Title = "Non-existent TodoItem", IsCompleted = false };

        // Act
        var response = await _client.PutAsJsonAsync("/todos/f3c1a4c2-9e7b-4c8e-9d7a-2b4f6e8c1a55", updatedTodo);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchTodo_UpdatesPartialTodo()
    {
        // Arrange
        await SetDatabase();
        var patchDoc = new
        {
            Title = "Learn ASP.NET Core - Patched"
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/todos/ca781d57-5ec2-45fd-8726-4157286f4889", patchDoc);
        var todo = await response.Content.ReadFromJsonAsync<TodoItem>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        todo.Should().NotBeNull();
        todo.Title.Should().Be(patchDoc.Title);
    }

    [Fact]
    public async Task PatchTodo_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        await SetDatabase();
        var patchDoc = new
        {
            Title = "Non-existent TodoItem - Patched"
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/todos/f3c1a4c2-9e7b-4c8e-9d7a-2b4f6e8c1a55", patchDoc);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}