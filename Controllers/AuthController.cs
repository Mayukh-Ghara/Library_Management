using LibraryWebAPI.Models;
using LibraryWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IJwtService jwtService;
        private readonly ILogger<AuthController> logger;

        public AuthController(IJwtService jwtService, ILogger<AuthController> logger)
        {
            this.jwtService = jwtService;
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            logger.LogInformation("Login attempt for user: {Username}", Username);
            if (Username == "admin" && Password == "password")
            {
                logger.LogInformation("Login successful for user: {Username}", Username);
                var token = jwtService.GenerateToken(Username,"Admin");
                return Ok(token);
            }
            else
            {
                logger.LogWarning("Login failed for user: {Username}", Username);
                return Unauthorized("Invalid username or password.");
            }

        }


    }
}
