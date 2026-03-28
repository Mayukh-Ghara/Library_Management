using Dapper;
using LibraryWebAPI.Models;
using LibraryWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Generators;

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

    // GET api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    // GET api/users/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
        {
            _logger.LogWarning("User with ID {Id} not found.", id);
            return NotFound(new { message = $"User with ID {id} not found." });
        }
        return Ok(user);
    }

    // POST api/users
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if email already exists
        var existing = await _userService.GetByEmailAsync(user.Email);
        if (existing is not null)
            return Conflict(new { message = "A user with this email already exists." });

        // Hash password before storing
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        var newId = await _userService.CreateAsync(user);

        _logger.LogInformation("User created with ID {Id}.", newId);
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId });
    }

    // PUT api/users/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _userService.GetByIdAsync(id);
        if (existing is null)
        {
            _logger.LogWarning("Update failed. User with ID {Id} not found.", id);
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        user.Id = id;
        user.PasswordHash = existing.PasswordHash; // Prevent overwriting password on update

        var updated = await _userService.UpdateAsync(user);
        if (!updated)
            return StatusCode(500, new { message = "Update failed unexpectedly." });

        _logger.LogInformation("User with ID {Id} updated.", id);
        return NoContent();
    }

    // DELETE api/users/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _userService.GetByIdAsync(id);
        if (existing is null)
        {
            _logger.LogWarning("Delete failed. User with ID {Id} not found.", id);
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        var deleted = await _userService.DeleteAsync(id);
        if (!deleted)
            return StatusCode(500, new { message = "Delete failed unexpectedly." });

        _logger.LogInformation("User with ID {Id} deleted.", id);
        return NoContent();
    }

    // PATCH api/users/5/deactivate
    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
            return NotFound(new { message = $"User with ID {id} not found." });

        user.IsActive = false;
        await _userService.UpdateAsync(user);

        _logger.LogInformation("User with ID {Id} deactivated.", id);
        return Ok(new { message = $"User {id} has been deactivated." });
    }

    // PATCH api/users/5/role
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

        _logger.LogInformation("User with ID {Id} role updated to {Role}.", id, request.Role);
        return Ok(new { message = $"User {id} role updated to '{request.Role}'." });
    }
}

// Small DTO for the role update endpoint
public record UpdateRoleRequest(string Role);