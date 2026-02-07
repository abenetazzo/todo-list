using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Todo.Api.Data;
using Todo.Api.Services;
using Todo.Api.Models;

namespace Todo.Api.Tests.Integration;

public class TodosEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static bool _seeded = false;

    public TodosEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        // Seed una volta per fixture
        if (!_seeded)
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.TodoItems.AddRange(
                new TodoItem { Id = 1, Title = "Learn ASP.NET Core", IsCompleted = false },
                new TodoItem { Id = 2, Title = "Build a Web API", IsCompleted = false },
                new TodoItem { Id = 3, Title = "Write Documentation", IsCompleted = true }
            );
            context.SaveChanges();
            _seeded = true;
        }
    }

    [Fact]
    public async Task GetTodos_ReturnsInitialList()
    {
        // Act
        var response = await _client.GetAsync("/todos");
        var todos = await response.Content.ReadFromJsonAsync<List<TodoItem>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        todos.Should().NotBeNull();
        todos!.Count.Should().Be(3);
    }
    
    [Fact]
    public async Task GetTodoById_ReturnsCorrectTodo()
    {
        // Arrange
        var expectedTodo = new TodoItem { Id = 1, Title = "Learn ASP.NET Core", IsCompleted = false };

        // Act
        var response = await _client.GetAsync("/todos/1");
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
        // Act
        var response = await _client.GetAsync("/todos/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PostTodo_CreatesNewTodo()
    {
        // Arrange
        var newTodo = new TodoItem { Title = "Write Integration Tests", IsCompleted = false };

        // Act
        var response = await _client.PostAsJsonAsync("/todos", newTodo);
        var createdTodo = await response.Content.ReadFromJsonAsync<TodoItem>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdTodo.Should().NotBeNull();
        createdTodo!.Id.Should().BeGreaterThan(0);
        createdTodo.Title.Should().Be(newTodo.Title);
        createdTodo.IsCompleted.Should().Be(newTodo.IsCompleted);
    }

    [Fact]
    public async Task DeleteTodo_ExistingId_ReturnsNoContent()
    {
        // Act
        var response = await _client.DeleteAsync("/todos/3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTodo_NonExistentId_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/todos/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PutTodo_UpdatesExistingTodo()
    {
        // Arrange
        var updatedTodo = new TodoItem { Id = 2, Title = "Learn ASP.NET Core - Updated", IsCompleted = true };

        // Act
        var response = await _client.PutAsJsonAsync("/todos/2", updatedTodo);
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
        var updatedTodo = new TodoItem { Id = 999, Title = "Non-existent TodoItem", IsCompleted = false };

        // Act
        var response = await _client.PutAsJsonAsync("/todos/999", updatedTodo);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchTodo_UpdatesPartialTodo()
    {
        // Arrange
        var patchDoc = new
        {
            Title = "Learn ASP.NET Core - Patched"
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/todos/2", patchDoc);
        var todo = await response.Content.ReadFromJsonAsync<TodoItem>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(2);
        todo.Title.Should().Be(patchDoc.Title);
    }

    [Fact]
    public async Task PatchTodo_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        var patchDoc = new
        {
            Title = "Non-existent TodoItem - Patched"
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/todos/999", patchDoc);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}