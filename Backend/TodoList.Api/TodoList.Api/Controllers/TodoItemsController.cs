using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TodoList.Api.DTOs;
using TodoList.Api.Services;

namespace TodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(ITodoService todoService, ILogger<TodoItemsController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var error = "An error occurred while retrieving the todo item.";

            try
            {
                var todos = await _todoService.GetTodosAsync();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, error);
                return StatusCode(500, new { messsage = error });
            }
        }

        // GET: api/TodoItems/...
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(int id)
        {
            var error = "Todo item not found.";

            try
            {
                var todo = await _todoService.GetTodoByIdAsync(id);
                return Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, error);
                return NotFound(new { message = error });
            }
        }

        // PUT: api/TodoItems/... 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItemDto todoItem)
        {
            var error = "An error occurred while updating the todo item.";

            try
            {
                await _todoService.UpdateTodoItemAsync(id, todoItem);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, error);
                return BadRequest(new { message = error });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, error);
                return StatusCode(500, new { message = error });
            }
        }

        // PUT: api/TodoItems/... 
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> MarkAsComplete(int id)
        {
            var error = "An error occurred while marking the todo item as complete.";

            try
            {
                await _todoService.MarkAsCompleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, error);
                return BadRequest(new { message = error });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, error);
                return StatusCode(500, new { message = error });
            }
        }

        // POST: api/TodoItems 
        [HttpPost]
        public async Task<IActionResult> PostTodoItem(TodoItemDto todoItem)
        {
            if (string.IsNullOrEmpty(todoItem?.Description))
            {
                return BadRequest("Description is required");
            }

            var error = "An error occurred while creating the todo item.";

            try
            {
                var createdDto = await _todoService.CreateTodoItemAsync(todoItem);
                return CreatedAtAction(nameof(GetTodos), new { id = createdDto.Id }, createdDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, error);
                return BadRequest(new { message = error });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, error);
                return StatusCode(500, new { message = error });
            }
        }
    }
}
