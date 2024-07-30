using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Api.Database.Models;

namespace TodoList.Api.Database.Repositories
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoItem>> GetTodosAsync();
        Task<TodoItem> GetTodoByIdAsync(int id);
        Task<TodoItem> GetTodoByNameAsync(string name);
        Task CreateTodoItemAsync(TodoItem todoItem);
        Task<bool> TodoItemExistAsync(int id);
        Task UpdateTodoItemAsync(TodoItem todoItem);
    }
}
