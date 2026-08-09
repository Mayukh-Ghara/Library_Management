using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryWebAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace LibraryWebAPI.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var key = _configuration["JwtSettings:SecretKey"]!;
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expiryDays = 7;

            // ADD THIS LINE: Capitalize the first letter of the role to match the Controller expectations
            string formattedRole = string.IsNullOrEmpty(user.Role)
                ? "User"
                : char.ToUpper(user.Role[0]) + user.Role.Substring(1).ToLower();

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name,           user.Username),
        new Claim(ClaimTypes.Email,          user.Email),
        
        // UPDATE THIS LINE: Use the newly formatted Title Case role
        new Claim(ClaimTypes.Role,           formattedRole),

        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expiryDays),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}