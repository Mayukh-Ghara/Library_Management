using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LibraryWebAPI.Services
{
    public interface IJwtService
    {
        string GenerateToken(string username, string roll);
    }
    public class JwtService : IJwtService
    {
        private readonly string key = "ThisIsMySuperSecretKey1234567ThisIsMySuperSecretKey1234567";

        public string GenerateToken(string username, string roll)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, roll)

        };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}


