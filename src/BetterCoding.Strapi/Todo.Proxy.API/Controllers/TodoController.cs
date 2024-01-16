using Microsoft.AspNetCore.Mvc;
using Todo.Proxy.Contracts;
using Todo.Proxy.Repository;

namespace Todo.Proxy.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<TodoController> _logger;

        public TodoController(ILogger<TodoController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTodosByUser")]
        public async Task<List<TodoEntry>> GetByWhoCreated(int userId)
        {
            var result = await new TodoEntryRepository().GetByWhoCreated(userId);
            return result;
        }
    }
}
