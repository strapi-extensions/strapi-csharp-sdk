using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo.Proxy.Contracts.API;
using Todo.Proxy.Repository;

namespace Todo.Proxy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("/login")]
        public async Task LogIn([FromBody] LogInRequest request)
        {
            await _authRepository.LogInAsync(request);
        }
    }
}
