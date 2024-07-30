namespace TodoList.Api.DTOs
{
    public class TodoItemDto
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }
    }
}
