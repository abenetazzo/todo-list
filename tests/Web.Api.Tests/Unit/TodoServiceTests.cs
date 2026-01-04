using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Web.Api.Models;
using Web.Api.Services;
using Web.Api;

namespace Web.Api.Tests.Unit;

public class TodoServiceTests
{
    private readonly TodoService _todoService = new();

    [Fact]
    public void GetAll_ReturnsInitialTodos()
    {
        // Act
        var todos = _todoService.GetAll();

        // Assert
        todos.Should().NotBeNull();
        todos.Count.Should().Be(3);
    }

    [Theory] [InlineData(1)] [InlineData(2)] [InlineData(3)]
    public void GetById_ExistingId_ReturnsTodoItem(int id)
    {
        // Act
        var todo = _todoService.GetById(id);

        // Assert
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(id);
    }

    [Theory] [InlineData(999)] [InlineData(-1)] [InlineData(0)]
    public void GetById_NonExistentId_ReturnsNull(int id)
    {
        // Act
        var todo = _todoService.GetById(id);

        // Assert
        todo.Should().BeNull();
    }

    [Fact]
    public void Create_AddsNewTodoItem()
    {
        // Arrange
        var newTodoDto = new CreateTodoItemDTO
        {
            Title = "New Todo",
            IsCompleted = false
        };

        // Act
        var createdTodo = _todoService.Create(newTodoDto);
        var todos = _todoService.GetAll();

        // Assert
        createdTodo.Should().NotBeNull();
        createdTodo.Id.Should().BeGreaterThan(0);
        createdTodo.Title.Should().Be(newTodoDto.Title);
        createdTodo.IsCompleted.Should().Be(newTodoDto.IsCompleted);
        todos.Count.Should().Be(4); // Initial 3 + 1 new
    }

    [Theory] [InlineData(1)] [InlineData(2)] [InlineData(3)]
    public void Delete_ExistingId_RemovesTodoItem(int id)
    {
        // Act
        var result = _todoService.Delete(id);
        var todo = _todoService.GetById(id);
        var todos = _todoService.GetAll();

        // Assert
        result.Should().BeTrue();
        todo.Should().BeNull();
        todos.Count.Should().Be(2); // Initial 3 - 1 deleted
    }

    [Theory] [InlineData(999)] [InlineData(-1)] [InlineData(0)]
    public void Delete_NonExistentId_ReturnsFalse(int id)
    {
        // Act
        var result = _todoService.Delete(id);

        // Assert
        result.Should().BeFalse();
    }

    [Theory] [InlineData(1)] [InlineData(2)] [InlineData(3)]
    public void Update_ExistingId_UpdatesTodoItem(int id)
    {
        // Arrange
        var updateDto = new UpdateTodoItemDTO
        {
            Title = "Updated Title",
            IsCompleted = true
        };

        // Act
        var updatedTodo = _todoService.Update(id, updateDto);
        var todo = _todoService.GetById(id);

        // Assert
        updatedTodo.Should().NotBeNull();
        updatedTodo!.Title.Should().Be(updateDto.Title);
        updatedTodo.IsCompleted.Should().Be(updateDto.IsCompleted);
    }

    [Theory] [InlineData(999)] [InlineData(-1)] [InlineData(0)]
    public void Update_NonExistentId_ReturnsNull(int id)
    {
        // Arrange
        var updateDto = new UpdateTodoItemDTO
        {
            Title = "Updated Title",
            IsCompleted = true
        };

        // Act
        var updatedTodo = _todoService.Update(id, updateDto);

        // Assert
        updatedTodo.Should().BeNull();
    }

    [Theory] [InlineData(1)] [InlineData(2)] [InlineData(3)]
    public void Patch_ExistingId_PartiallyUpdatesTodoItem(int id)
    {
        // Arrange
        var patchDto = new PatchTodoItemDTO
        {
            Title = "Patched Title"
            // IsCompleted is not set
        };
        var originalTodo = _todoService.GetById(id);

        // Act
        var patchedTodo = _todoService.Patch(id, patchDto);
        var todo = _todoService.GetById(id);

        // Assert
        patchedTodo.Should().NotBeNull();
        patchedTodo!.Title.Should().Be(patchDto.Title);
        patchedTodo.IsCompleted.Should().Be(originalTodo!.IsCompleted); // Should remain unchanged
    }

    [Theory] [InlineData(999)] [InlineData(-1)] [InlineData(0)]
    public void Patch_NonExistentId_ReturnsNull(int id)
    {
        // Arrange
        var patchDto = new PatchTodoItemDTO
        {
            Title = "Patched Title"
        };

        // Act
        var patchedTodo = _todoService.Patch(id, patchDto);

        // Assert
        patchedTodo.Should().BeNull();
    }
}