using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;
using Todo.Domain.Todos;
using Todo.Api.Services;
using Todo.Api.Data;

namespace Todo.Api.Tests.Unit;

public class TodoServiceTests
{
    private readonly AppDbContext _context;
    private readonly ITodoService _todoService;

    public TodoServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        // Seed data
        _context.TodoItems.AddRange(
            new TodoItem { Id = new Guid("2d41d56a-ac21-475d-8fbc-62d54f10ce94"), Title = "Primo todoItem", IsCompleted = false },
            new TodoItem { Id = new Guid("ca781d57-5ec2-45fd-8726-4157286f4889"), Title = "Secondo todoItem", IsCompleted = false },
            new TodoItem { Id = new Guid("3ddcf8cc-dabb-4492-ab41-e81d54fd05c2"), Title = "Test finale ok", IsCompleted = true }
        );
        _context.SaveChanges();

        _todoService = new TodoService(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsInitialTodos()
    {
        // Act
        var todos = await _todoService.GetAllAsync();

        // Assert
        todos.Should().NotBeNull();
        todos.Count.Should().Be(3);
    }

    [Theory]
    [InlineData("2d41d56a-ac21-475d-8fbc-62d54f10ce94")]
    [InlineData("ca781d57-5ec2-45fd-8726-4157286f4889")]
    [InlineData("3ddcf8cc-dabb-4492-ab41-e81d54fd05c2")]
    public async Task GetByIdAsync_ExistingId_ReturnsTodoItem(Guid id)
    {
        // Act
        var todo = await _todoService.GetByIdAsync(id);

        // Assert
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(id);
    }

    [Theory] [InlineData("f3c1a4c2-9e7b-4c8e-9d7a-2b4f6e8c1a55")]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull(Guid id)
    {
        // Act
        var todo = await _todoService.GetByIdAsync(id);

        // Assert
        todo.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_AddsNewTodoItem()
    {
        // Arrange
        var newTodoDto = new CreateTodoItemDTO
        {
            Title = "New Todo",
            IsCompleted = false
        };

        // Act
        var createdTodo = await _todoService.CreateAsync(newTodoDto);
        var todos = await _todoService.GetAllAsync();

        // Assert
        createdTodo.Should().NotBeNull();
        createdTodo.Title.Should().Be(newTodoDto.Title);
        createdTodo.IsCompleted.Should().Be(newTodoDto.IsCompleted);
        todos.Count.Should().Be(4); // Initial 3 + 1 new
    }

    [Theory]
    [InlineData("2d41d56a-ac21-475d-8fbc-62d54f10ce94")]
    [InlineData("ca781d57-5ec2-45fd-8726-4157286f4889")]
    [InlineData("3ddcf8cc-dabb-4492-ab41-e81d54fd05c2")]
    public async Task DeleteAsync_ExistingId_RemovesTodoItem(Guid id)
    {
        // Act
        var result = await _todoService.DeleteAsync(id);
        var todo = await _todoService.GetByIdAsync(id);
        var todos = await _todoService.GetAllAsync();

        // Assert
        result.Should().BeTrue();
        todo.Should().BeNull();
        todos.Count.Should().Be(2); // Initial 3 - 1 deleted
    }

    [Theory] [InlineData("f3c1a4c2-9e7b-4c8e-9d7a-2b4f6e8c1a55")]
    public async Task DeleteAsync_NonExistentId_ReturnsFalse(Guid id)
    {
        // Act
        var result = await _todoService.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("2d41d56a-ac21-475d-8fbc-62d54f10ce94")]
    [InlineData("ca781d57-5ec2-45fd-8726-4157286f4889")]
    [InlineData("3ddcf8cc-dabb-4492-ab41-e81d54fd05c2")]
    public async Task UpdateAsync_ExistingId_UpdatesTodoItem(Guid id)
    {
        // Arrange
        var updateDto = new UpdateTodoItemDTO
        {
            Title = "Updated Title",
            IsCompleted = true
        };

        // Act
        var updatedTodo = await _todoService.UpdateAsync(id, updateDto);
        var todo = await _todoService.GetByIdAsync(id);

        // Assert
        updatedTodo.Should().NotBeNull();
        updatedTodo!.Title.Should().Be(updateDto.Title);
        updatedTodo.IsCompleted.Should().Be(updateDto.IsCompleted);
    }

    [Theory] [InlineData("f3c1a4c2-9e7b-4c8e-9d7a-2b4f6e8c1a55")]
    public async Task UpdateAsync_NonExistentId_ReturnsNull(Guid id)
    {
        // Arrange
        var updateDto = new UpdateTodoItemDTO
        {
            Title = "Updated Title",
            IsCompleted = true
        };

        // Act
        var updatedTodo = await _todoService.UpdateAsync(id, updateDto);

        // Assert
        updatedTodo.Should().BeNull();
    }

    [Theory]
    [InlineData("2d41d56a-ac21-475d-8fbc-62d54f10ce94")]
    [InlineData("ca781d57-5ec2-45fd-8726-4157286f4889")]
    [InlineData("3ddcf8cc-dabb-4492-ab41-e81d54fd05c2")]
    public async Task PatchAsync_ExistingId_PartiallyUpdatesTodoItem(Guid id)
    {
        // Arrange
        var patchDto = new PatchTodoItemDTO
        {
            Title = "Patched Title"
            // IsCompleted is not set
        };
        var originalTodo = await _todoService.GetByIdAsync(id);

        // Act
        var patchedTodo = await _todoService.PatchAsync(id, patchDto);
        var todo = await _todoService.GetByIdAsync(id);

        // Assert
        patchedTodo.Should().NotBeNull();
        patchedTodo!.Title.Should().Be(patchDto.Title);
        patchedTodo.IsCompleted.Should().Be(originalTodo!.IsCompleted); // Should remain unchanged
    }

    [Theory] [InlineData("f3c1a4c2-9e7b-4c8e-9d7a-2b4f6e8c1a55")]
    public async Task PatchAsync_NonExistentId_ReturnsNull(Guid id)
    {
        // Arrange
        var patchDto = new PatchTodoItemDTO
        {
            Title = "Patched Title"
        };

        // Act
        var patchedTodo = await _todoService.PatchAsync(id, patchDto);

        // Assert
        patchedTodo.Should().BeNull();
    }
}