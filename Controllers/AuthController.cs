using LibraryWebAPI.DTOs;
using LibraryWebAPI.Models;
using LibraryWebAPI.Data;
using LibraryWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase  // ✅ ControllerBase not Controller
    {
        private readonly IJwtService _jwtService;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IJwtService jwtService, AppDbContext context, ILogger<AuthController> logger)
        {
            _jwtService = jwtService;
            _context = context;
            _logger = logger;
        }

        // ─── REGISTER ─────────────────────────────────────────
        // POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            _logger.LogInformation("Register attempt for: {Email}", dto.Email);

            // Check duplicate email
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (emailExists)
                return BadRequest(new { message = "Email is already registered." });

            // Check duplicate username
            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == dto.Username);

            if (usernameExists)
                return BadRequest(new { message = "Username is already taken." });

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                Role = "user",
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User registered successfully: {Email}", dto.Email);
            return Ok(new { message = "Registration successful." });
        }

        // ─── LOGIN ────────────────────────────────────────────
        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)  // ✅ [FromBody] not query params
        {
            _logger.LogInformation("Login attempt for: {Email}", dto.Email);

            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);

            if (user == null)
            {
                _logger.LogWarning("Login failed — user not found: {Email}", dto.Email);
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Verify password
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed — wrong password: {Email}", dto.Email);
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Generate token
            var token = _jwtService.GenerateToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            _logger.LogInformation("Login successful: {Email}", dto.Email);

            return Ok(new
            {
                message = "Login successful.",
                data = new AuthResponseDto
                {
                    Token = token,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    ExpiresAt = expiresAt
                }
            });
        }
    }
}