using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Api.DTOs;

namespace TodoList.Api.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItemDto>> GetTodosAsync();
        Task<TodoItemDto> GetTodoByIdAsync(int id);
        Task<TodoItemDto> CreateTodoItemAsync(TodoItemDto todoItemDto);
        Task UpdateTodoItemAsync(int id, TodoItemDto todoItemDto);
        Task MarkAsCompleteAsync(int id);
    }
}
