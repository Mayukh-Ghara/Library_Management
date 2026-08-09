using System.Security.Claims;

namespace LibraryWebAPI.Services
{
    /// <summary>
    /// Service to extract user information from JWT authorization token claims.
    /// Provides access to user identity data from the HttpContext.User claims.
    /// </summary>
    public interface IUsrTokenContext
    {
        int GetUserId();
        string GetUsername();
        string GetEmail();
        string GetRole();
        bool TryGetUserId(out int userId);
        bool TryGetUsername(out string username);
        bool TryGetEmail(out string email);
        bool TryGetRole(out string role);
    }

    public class UsrTokenContext : IUsrTokenContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UsrTokenContext> _logger;

        public UsrTokenContext(IHttpContextAccessor httpContextAccessor, ILogger<UsrTokenContext> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        /// <summary>
        /// Gets the user ID from the NameIdentifier claim.
        /// </summary>
        /// <returns>User ID if found</returns>
        /// <exception cref="InvalidOperationException">Thrown when user is not authenticated or NameIdentifier claim is missing</exception>
        public int GetUserId()
        {
            if (!TryGetUserId(out var userId))
            {
                _logger.LogWarning("Failed to extract user ID from token");
                throw new InvalidOperationException("User is not authenticated or NameIdentifier claim is missing.");
            }
            return userId;
        }

        /// <summary>
        /// Gets the username from the Name claim.
        /// </summary>
        /// <returns>Username if found</returns>
        /// <exception cref="InvalidOperationException">Thrown when username claim is missing</exception>
        public string GetUsername()
        {
            if (!TryGetUsername(out var username))
            {
                _logger.LogWarning("Failed to extract username from token");
                throw new InvalidOperationException("Username claim is missing from token.");
            }
            return username;
        }

        /// <summary>
        /// Gets the email from the Email claim.
        /// </summary>
        /// <returns>Email if found</returns>
        /// <exception cref="InvalidOperationException">Thrown when email claim is missing</exception>
        public string GetEmail()
        {
            if (!TryGetEmail(out var email))
            {
                _logger.LogWarning("Failed to extract email from token");
                throw new InvalidOperationException("Email claim is missing from token.");
            }
            return email;
        }

        /// <summary>
        /// Gets the role from the Role claim.
        /// </summary>
        /// <returns>Role if found</returns>
        /// <exception cref="InvalidOperationException">Thrown when role claim is missing</exception>
        public string GetRole()
        {
            if (!TryGetRole(out var role))
            {
                _logger.LogWarning("Failed to extract role from token");
                throw new InvalidOperationException("Role claim is missing from token.");
            }
            return role;
        }

        /// <summary>
        /// Gets the JWT ID (Jti) claim which uniquely identifies the token.
        /// </summary>
        /// <returns>Jti value if found, null otherwise</returns>
        

        /// <summary>
        /// Attempts to get the user ID from the NameIdentifier claim.
        /// </summary>
        /// <param name="userId">The extracted user ID</param>
        /// <returns>True if user ID was successfully extracted, false otherwise</returns>
        public bool TryGetUserId(out int userId)
        {
            userId = 0;

            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return false;
            }

            return int.TryParse(userIdClaim, out userId);
        }

        /// <summary>
        /// Attempts to get the username from the Name claim.
        /// </summary>
        /// <param name="username">The extracted username</param>
        /// <returns>True if username was successfully extracted, false otherwise</returns>
        public bool TryGetUsername(out string username)
        {
            username = User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            return !string.IsNullOrEmpty(username);
        }

        /// <summary>
        /// Attempts to get the email from the Email claim.
        /// </summary>
        /// <param name="email">The extracted email</param>
        /// <returns>True if email was successfully extracted, false otherwise</returns>
        public bool TryGetEmail(out string email)
        {
            email = User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            return !string.IsNullOrEmpty(email);
        }

        /// <summary>
        /// Attempts to get the role from the Role claim.
        /// </summary>
        /// <param name="role">The extracted role</param>
        /// <returns>True if role was successfully extracted, false otherwise</returns>
        public bool TryGetRole(out string role)
        {
            role = User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            return !string.IsNullOrEmpty(role);
        }
    }
}
