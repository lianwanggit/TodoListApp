using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Api.Database.Models;

namespace TodoList.Api.Database.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoContext _context;

        public TodoRepository(TodoContext todoContext)
        {
            _context = todoContext;
        }

        public async Task CreateTodoItemAsync(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
        }

        public async Task<TodoItem> GetTodoByIdAsync(int id) =>
            await _context.TodoItems.FindAsync(id);

        public async Task<TodoItem> GetTodoByNameAsync(string name) =>
            await _context.TodoItems.FirstOrDefaultAsync(x => x.Description.Equals(name, StringComparison.OrdinalIgnoreCase));

        public async Task<IEnumerable<TodoItem>> GetTodosAsync() =>
            await _context.TodoItems.ToListAsync();

        public async Task<bool> TodoItemExistAsync(int id) =>
            await _context.TodoItems.AnyAsync(x => x.Id == id);

        public async Task UpdateTodoItemAsync(TodoItem todoItem)
        {
            _context.Entry(todoItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
