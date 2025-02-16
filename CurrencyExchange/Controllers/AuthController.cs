using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using CurrencyExchange.BusinessModels.Model;
using Swashbuckle.AspNetCore.Annotations;


namespace CurrencyExchange.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [SwaggerOperation(
  Summary = "Get Token",
  Description = "Two Addount admin and User Credentials are  Shared"
)]

        [HttpPost("login")]
        public IActionResult Login([FromBody] BusinessModels.Model.LoginRequest request)
        {
            if (request.Username == "admin" && request.Password == "admin123")
            {
                var token = _jwtService.GenerateToken("Admin", request.Username);
                return Ok(new { Token = token });
            }
            else if(request.Username == "user" && request.Password == "user123")
            {
                var token = _jwtService.GenerateToken("User", request.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }
    }
}
