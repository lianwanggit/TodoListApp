using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.Database.Models;
using TodoList.Api.Database.Repositories;
using TodoList.Api.DTOs;

namespace TodoList.Api.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;

        public TodoService(ITodoRepository repository) 
        {
            _repository = repository;
        }

        public async Task<TodoItemDto> CreateTodoItemAsync(TodoItemDto todoItemDto)
        {
            var existingTodo = await _repository.GetTodoByNameAsync(todoItemDto.Description);
            if (existingTodo != null) 
            {
                throw new InvalidOperationException("A todo item with the same description already exists.");
            }

            var todoItem = new TodoItem
            {
                Description = todoItemDto.Description,
                IsCompleted = todoItemDto.IsCompleted
            };

            await _repository.CreateTodoItemAsync(todoItem);

            todoItemDto.Id = todoItem.Id;
            return todoItemDto;
        }

        public async Task<TodoItemDto> GetTodoByIdAsync(int id)
        {
            var todo = await _repository.GetTodoByIdAsync(id);
            if (todo == null)
            {
                throw new Exception("Todo item not found.");
            }

            return new TodoItemDto
            {
                Id = todo.Id,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted
            };
        }

        public async Task<IEnumerable<TodoItemDto>> GetTodosAsync()
        {
            var todos = await _repository.GetTodosAsync();
            return todos.Select(todo => new TodoItemDto
            {
                Id = todo.Id,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted
            }).ToList();
        }

        public async Task MarkAsCompleteAsync(int id)
        {
            var todoItem = await _repository.GetTodoByIdAsync(id);
            if (todoItem == null) 
            {
                throw new Exception("Todo item not found.");
            }

            if (todoItem.IsCompleted)
            {
                throw new InvalidOperationException("Todo item is already marked as complete.");
            }

            todoItem.IsCompleted = true;
            await _repository.UpdateTodoItemAsync(todoItem);
        }

        public async Task UpdateTodoItemAsync(int id, TodoItemDto todoItemDto)
        {
            if (!await _repository.TodoItemExistAsync(id))
            {
                throw new Exception("Todo item not found.");
            }

            var existingTodo = await _repository.GetTodoByNameAsync(todoItemDto.Description);
            if (existingTodo != null && existingTodo.Id != id)
            {
                throw new InvalidOperationException("A todo item with the same name already exist.");
            }

            var todoItem = new TodoItem
            {
                Id = id,
                Description = todoItemDto.Description,
                IsCompleted = todoItemDto.IsCompleted
            };

            await _repository.UpdateTodoItemAsync(todoItem);
        }
    }
}
