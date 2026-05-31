using LibraryWebAPI.Models;
using LibraryWebAPI.DTOs; 
using LibraryWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();

        // Map domain items to Response DTOs safely skipping password hashes
        var response = users.Select(u => MapToResponseDto(u));
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
        {
            _logger.LogWarning("User with ID {Id} not found.", id);
            return NotFound(new { message = $"User with ID {id} not found." });
        }
        return Ok(MapToResponseDto(user));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _userService.GetByEmailAsync(dto.Email);
        if (existing is not null)
            return Conflict(new { message = "A user with this email already exists." });

        // Build User Object safely mapping inbound plain password to hashed equivalent
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            IsActive = true,
            Role = "user"
        };

        var newId = await _userService.CreateAsync(user);

        _logger.LogInformation("User created with ID {Id}.", newId);
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _userService.GetByIdAsync(id);
        if (existing is null)
        {
            _logger.LogWarning("Update failed. User with ID {Id} not found.", id);
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        // Maintain password hash from existing record
        existing.Username = dto.Username;
        existing.Email = dto.Email;
        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Phone = dto.Phone;
        existing.IsActive = dto.IsActive;
        existing.Role = dto.Role;

        var updated = await _userService.UpdateAsync(existing);
        if (!updated)
            return StatusCode(500, new { message = "Update failed unexpectedly." });

        _logger.LogInformation("User with ID {Id} updated.", id);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _userService.GetByIdAsync(id);
        if (existing is null)
            return NotFound(new { message = $"User with ID {id} not found." });

        var deleted = await _userService.DeleteAsync(id);
        if (!deleted)
            return StatusCode(500, new { message = "Delete failed unexpectedly." });

        return NoContent();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
            return NotFound(new { message = $"User with ID {id} not found." });

        user.IsActive = false;
        await _userService.UpdateAsync(user);

        return Ok(new { message = $"User {id} has been deactivated." });
    }

    [HttpPatch("{id:int}/role")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
    {
        var validRoles = new[] { "user", "admin", "librarian" };
        if (!validRoles.Contains(request.Role))
            return BadRequest(new { message = $"Invalid role. Allowed: {string.Join(", ", validRoles)}" });

        var user = await _userService.GetByIdAsync(id);
        if (user is null)
            return NotFound(new { message = $"User with ID {id} not found." });

        user.Role = request.Role;
        await _userService.UpdateAsync(user);

        return Ok(new { message = $"User {id} role updated to '{request.Role}'." });
    }

    // Helper mapper to keep controllers cleaner (or use AutoMapper)
    private static UserResponseDto MapToResponseDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Phone = user.Phone,
        IsActive = user.IsActive,
        Role = user.Role
    };
}