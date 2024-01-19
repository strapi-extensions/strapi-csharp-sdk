using Microsoft.AspNetCore.Mvc;
using Todo.Proxy.Contracts;
using Todo.Proxy.Contracts.API;
using Todo.Proxy.Repository;

namespace Todo.Proxy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ILogger<TodoController> _logger;
        private readonly ITodoEntryRepository _todoEntryRepository;

        public TodoController(ILogger<TodoController> logger,
            ITodoEntryRepository todoEntryRepository)
        {
            _logger = logger;
            _todoEntryRepository = todoEntryRepository;
        }

        [HttpGet("/id", Name = "GetById")]
        public async Task<TodoEntry> CreateOne(int id)
        {
            var result = await _todoEntryRepository.GetById(id);
            return result;
        }

        [HttpGet(Name = "GetTodosByUser")]
        public async Task<List<TodoEntry>> GetByWhoCreated(int userId)
        {
            var result = await _todoEntryRepository.GetByWhoCreated(userId);
            return result;
        }
    }
}
