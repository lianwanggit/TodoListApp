using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.Database;
using TodoList.Api.Database.Models;
using TodoList.Api.Database.Repositories;
using TodoList.Api.DTOs;
using TodoList.Api.Services;
using Xunit;

namespace TodoList.Api.UnitTests
{
    public class TodoServiceTests
    {
        private readonly Mock<ITodoRepository> _todoRepositoryMock;
        private readonly TodoService _todoService;

        public TodoServiceTests()
        {
            _todoRepositoryMock = new Mock<ITodoRepository>();
            _todoService = new TodoService(_todoRepositoryMock.Object);

            //// Alternatively, use in-memory database
            //var options = new DbContextOptionsBuilder<TodoContext>()
            //    .UseInMemoryDatabase(databaseName: "TodoListTest")
            //    .Options;
            //var context = new TodoContext(options);
            //var todoRepository = new TodoRepository(context);
        }

        [Fact]
        public async Task GetTodosAsync_ReturnsAllTodos()
        {
            // Arrange
            var todos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Description = "Test Todo 1", IsCompleted = false },
                new TodoItem { Id = 2, Description = "Test Todo 2", IsCompleted = true }
            };
            _todoRepositoryMock.Setup(repo => repo.GetTodosAsync()).ReturnsAsync(todos);

            // Act
            var result = await _todoService.GetTodosAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Test Todo 1", result.First().Description);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ReturnsTodo_WhenTodoExists()
        {
            // Arrange
            var todo = new TodoItem { Id = 1, Description = "Test Todo", IsCompleted = false };
            _todoRepositoryMock.Setup(repo => repo.GetTodoByIdAsync(1)).ReturnsAsync(todo);

            // Act
            var result = await _todoService.GetTodoByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Todo", result.Description);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ThrowException_WhenTodoDoesNotExist()
        {
            // Arrange
            _todoRepositoryMock.Setup(repo => repo.GetTodoByIdAsync(1)).ReturnsAsync((TodoItem)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _todoService.GetTodoByIdAsync(1));
        }

        [Fact]
        public async Task CreateTodoAsync_CreatesTodo_WhenNameIsUnique()
        {
            // Arrange
            var todoDto = new TodoItemDto { Description = "New Todo" };
            _todoRepositoryMock.Setup(repo => repo.GetTodoByNameAsync("New Todo")).ReturnsAsync((TodoItem)null);

            // Act
            var result = await _todoService.CreateTodoItemAsync(todoDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Todo", result.Description);
            _todoRepositoryMock.Verify(repo => repo.CreateTodoItemAsync(It.IsAny<TodoItem>()), Times.Once);
        }

        [Fact]
        public async Task CreateTodoAsync_ThrowException_WhenNameIsNotUnique()
        {
            // Arrange
            var existingTodo = new TodoItem { Id = 1, Description = "Existing Todo" };
            var todoDto = new TodoItemDto { Description = "Existing Todo" };
            _todoRepositoryMock.Setup(repo => repo.GetTodoByNameAsync("Existing Todo")).ReturnsAsync(existingTodo);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _todoService.CreateTodoItemAsync(todoDto));
        }

        [Fact]
        public async Task UpdateTodoAsync_UpdatesTodo_WhenTodoExistsAndNameIsUnique()
        {
            // Arrange
            var existingTodo = new TodoItem { Id = 1, Description = "Existing Todo", IsCompleted = false };
            var todoDto = new TodoItemDto { Description = "Updated Todo", IsCompleted = true };
            _todoRepositoryMock.Setup(repo => repo.TodoItemExistAsync(1)).ReturnsAsync(true);
            _todoRepositoryMock.Setup(repo => repo.GetTodoByNameAsync("Updated Todo")).ReturnsAsync((TodoItem)null);

            // Act
            await _todoService.UpdateTodoItemAsync(1, todoDto);

            // Assert
            _todoRepositoryMock.Verify(repo => repo.UpdateTodoItemAsync(It.IsAny<TodoItem>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTodoAsync_ThrowException_WhenTodoDoesNotExist()
        {
            // Arrange
            var todoDto = new TodoItemDto { Description = "Updated Todo", IsCompleted = true };
            _todoRepositoryMock.Setup(repo => repo.TodoItemExistAsync(1)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _todoService.UpdateTodoItemAsync(1, todoDto));
        }

        [Fact]
        public async Task UpdateTodoAsync_ThrowException_WhenNameIsNotUnique()
        {
            // Arrange
            var existingTodo = new TodoItem { Id = 2, Description = "Updated Todo", IsCompleted = true };
            var todoDto = new TodoItemDto { Description = "Updated Todo" };
            _todoRepositoryMock.Setup(repo => repo.TodoItemExistAsync(1)).ReturnsAsync(true);
            _todoRepositoryMock.Setup(repo => repo.GetTodoByNameAsync("Updated Todo")).ReturnsAsync(existingTodo);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _todoService.UpdateTodoItemAsync(1, todoDto));
        }

        [Fact]
        public async Task MaskAsCompleteAsync_MarksTodoAsComplete_WhenTodoExistsAndIsNotComplete()
        {
            // Arrange
            var todo = new TodoItem { Id = 1, Description = "Test Todo", IsCompleted = false };
            _todoRepositoryMock.Setup(repo => repo.GetTodoByIdAsync(1)).ReturnsAsync(todo);

            // Act
            await _todoService.MarkAsCompleteAsync(1);

            // Assert
            _todoRepositoryMock.Verify(repo => repo.UpdateTodoItemAsync(It.Is<TodoItem>(t => t.IsCompleted)), Times.Once);
        }

        [Fact]
        public async Task MaskAsCompleteAsync_ThrowException_WhenTodoDoesNotExist()
        {
            // Arrange
            _todoRepositoryMock.Setup(repo => repo.GetTodoByIdAsync(1)).ReturnsAsync((TodoItem)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _todoService.MarkAsCompleteAsync(1));
        }

        [Fact]
        public async Task MaskAsCompleteAsync_ThrowException_WhenTodoIsAlreadyComplete()
        {
            // Arrange
            var todo = new TodoItem { Id = 1, Description = "Test Todo", IsCompleted = true };
            _todoRepositoryMock.Setup(repo => repo.GetTodoByIdAsync(1)).ReturnsAsync(todo);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _todoService.MarkAsCompleteAsync(1));
        }
    }
}
