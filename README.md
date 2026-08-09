# Library Management API

A comprehensive REST API for managing a digital library system built with **ASP.NET Core 8**, featuring user authentication, book management, borrowing operations, and book reviews.

**Repository**: [Library_Management](https://github.com/Mayukh-Ghara/Library_Management)  
**Branch**: `feature/dapper-users`

---

## ?? Table of Contents

- [Features](#-features)
- [Technology Stack](#-technology-stack)
- [Project Structure](#-project-structure)
- [Installation & Setup](#-installation--setup)
- [Configuration](#-configuration)
- [API Endpoints](#-api-endpoints)
- [Database Schema](#-database-schema)
- [Authentication & Authorization](#-authentication--authorization)
- [Service Layer](#-service-layer)
- [Error Handling](#-error-handling)
- [CORS Configuration](#-cors-configuration)
- [Development](#-development)
- [Contributing](#-contributing)

---

## ?? Features

### Authentication & Authorization
- **User Registration**: Create new user accounts with email validation
- **JWT Authentication**: Secure token-based authentication with expiration
- **Role-Based Access Control**: Support for Admin and User roles
- **Password Security**: BCrypt password hashing for secure storage
- **Token Context Extraction**: `UsrTokenContext` service for extracting user data from tokens

### Book Management
- **Browse Books**: Search and paginate through available books
- **Book Details**: Retrieve detailed information about individual books
- **Copy Management**: Track available copies and borrowing status
- **Admin Operations**: Create and manage book inventory (Admin only)

### Borrowing System
- **Borrow Books**: Users can borrow available books with customizable loan periods
- **Return Books**: Track book returns with automatic status updates
- **Borrowing History**: View all borrowing records and status
- **Overdue Tracking**: Monitor overdue books and due dates
- **Availability Checks**: Prevent duplicate borrowing and track inventory

### Reviews & Ratings
- **Create Reviews**: Users can leave reviews and ratings for books
- **Update Reviews**: Edit existing reviews
- **Delete Reviews**: Remove reviews (owner or admin can delete)
- **View Reviews**: Browse all reviews for a specific book
- **Public Access**: Anonymous users can view book reviews

---

## ?? Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| **Framework** | ASP.NET Core | 8.0 |
| **Language** | C# | Latest (.NET 8) |
| **Database** | MySQL | 5.7+ |
| **ORM** | Entity Framework Core | 8.0.5 |
| **Authentication** | JWT Bearer | 8.0.25 |
| **Data Access** | Dapper | 2.1.72 |
| **Validation** | FluentValidation | 12.1.1 |
| **Password Hashing** | BCrypt.Net-Next | 4.1.0 |
| **API Documentation** | Swagger/Swashbuckle | 10.1.4 |

---

## ?? Project Structure

```
LibraryWebAPI/
??? Controllers/              # API endpoints
?   ??? AuthController.cs     # Authentication (Register, Login)
?   ??? BooksController.cs    # Book management and search
?   ??? BorrowingsController.cs # Book borrowing operations
?   ??? ReviewsController.cs  # Book reviews and ratings
?   ??? UsersController.cs    # User management
?
??? Models/                   # Domain entities
?   ??? User.cs              # User model
?   ??? Book.cs              # Book model
?   ??? Borrowing.cs         # Borrowing transaction
?   ??? Review.cs            # User reviews
?   ??? BookCount.cs         # Helper model for book count queries
?
??? Services/                 # Business logic
?   ??? AuthService.cs       # Authentication logic (if exists)
?   ??? BookService.cs       # Book operations
?   ??? BorrowingService.cs  # Borrowing logic with transactions
?   ??? ReviewService.cs     # Review management
?   ??? UserService.cs       # User operations
?   ??? JwtService.cs        # JWT token generation
?   ??? UsrTokenContext.cs   # Token claim extraction
?
??? DTOs/                     # Data Transfer Objects
?   ??? AuthResponseDto.cs   # Login response
?   ??? LoginDto.cs          # Login request
?   ??? RegisterDto.cs       # Registration request
?   ??? BorrowRequestDto.cs  # Borrow request
?   ??? BorrowingResponseDto.cs # Borrow response
?   ??? ReturnRequestDto.cs  # Return request
?   ??? CreateReviewDto.cs   # Create review request
?   ??? UpdateReviewDto.cs   # Update review request
?   ??? ReviewResponseDto.cs # Review response
?   ??? UserDtos.cs          # User-related DTOs
?   ??? BookReviewSummaryDto.cs # Book with reviews
?   ??? PagedResult.cs       # Pagination wrapper
?
??? Data/                     # Database context
?   ??? AppDbContext.cs      # EF Core DbContext
?   ??? UserDbContext.cs     # ADO.NET context for Dapper
?
??? Validators/              # FluentValidation validators
?   ??? BookValidator.cs     # Book validation rules
?   ??? BorrowingValidator.cs # Borrowing validation rules
?
??? Program.cs               # Application startup configuration
??? LibraryWebAPI.csproj     # Project file with dependencies
```

---

## ?? Installation & Setup

### Prerequisites

- .NET 8 SDK or later
- MySQL Server 5.7+
- Visual Studio 2022 or Visual Studio Code
- Git

### Step 1: Clone the Repository

```bash
git clone https://github.com/Mayukh-Ghara/Library_Management.git
cd LibraryWebAPI
```

### Step 2: Restore Dependencies

```bash
dotnet restore
```

### Step 3: Configure Database Connection

Create or update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LibraryDB;User=root;Password=your_password;Port=3306;"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-characters-long",
    "Issuer": "LibraryAPI",
    "Audience": "LibraryClient"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Step 4: Apply Database Migrations

```bash
dotnet ef database update
```

### Step 5: Run the Application

```bash
dotnet run
```

The API will be available at `https://localhost:5001` by default.

### Step 6: Access Swagger Documentation

Navigate to `https://localhost:5001/swagger` to explore the API interactively.

---

## ?? Configuration

### JWT Settings

Configure JWT authentication in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-very-long-secret-key-for-jwt-signing-min-32-chars",
    "Issuer": "LibraryAPI",
    "Audience": "LibraryClient",
    "ExpiryDays": 7
  }
}
```

**Important**: 
- `SecretKey` must be at least 32 characters long
- Use a strong, random key in production
- Store secrets in Azure Key Vault or similar in production

### Database Connection

Update `DefaultConnection` in `appsettings.json`:

```
Server=localhost;Database=LibraryDB;User=root;Password=password;Port=3306;
```

### CORS Configuration

CORS is configured to allow the Angular frontend:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

**To modify**: Update `Program.cs` with your frontend URL.

---

## ?? API Endpoints

### Authentication (`/api/auth`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/register` | Register a new user | ? No |
| POST | `/login` | Authenticate user and get JWT | ? No |

### Books (`/api/books`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/` | Get paginated list of books with search | ? Yes |
| GET | `/{id}` | Get book details by ID | ? Yes |
| POST | `/` | Create a new book | ? Admin |
| PUT | `/{id}` | Update book details | ? Admin |
| DELETE | `/{id}` | Delete a book | ? Admin |

**Query Parameters for GET `/`**:
- `search` (optional): Search by title or author
- `page` (optional, default: 1): Page number
- `pageSize` (optional, default: 6): Items per page

### Borrowings (`/api/borrowings`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/borrow` | Borrow a book | ? User |
| POST | `/return` | Return a borrowed book | ? User |
| GET | `/` | Get user's borrowing history | ? User |

### Reviews (`/api/reviews`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/book/{bookId}` | Create a review for a book | ? User |
| PUT | `/{reviewId}` | Update a review | ? User |
| DELETE | `/{reviewId}` | Delete a review | ? User/Admin |
| GET | `/book/{bookId}` | Get all reviews for a book | ? No |
| GET | `/my-reviews` | Get user's own reviews | ? User |

### Users (`/api/users`)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/{userId}` | Get user profile | ? User |
| PUT | `/{userId}` | Update user profile | ? User |

---

## ??? Database Schema

### Users Table

```sql
CREATE TABLE users (
  id INT PRIMARY KEY AUTO_INCREMENT,
  username VARCHAR(100) UNIQUE NOT NULL,
  email VARCHAR(100) UNIQUE NOT NULL,
  password_hash VARCHAR(255) NOT NULL,
  first_name VARCHAR(100),
  last_name VARCHAR(100),
  phone VARCHAR(20),
  is_active BOOLEAN DEFAULT true,
  role VARCHAR(50) DEFAULT 'user'
);
```

### Books Table

```sql
CREATE TABLE books (
  id INT PRIMARY KEY AUTO_INCREMENT,
  title VARCHAR(200) NOT NULL,
  author VARCHAR(150) NOT NULL,
  isbn VARCHAR(20) UNIQUE NOT NULL,
  published_year INT,
  copies_available INT DEFAULT 0
);
```

### Borrowings Table

```sql
CREATE TABLE borrowings (
  id INT PRIMARY KEY AUTO_INCREMENT,
  user_id INT NOT NULL,
  book_id INT NOT NULL,
  borrowed_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  due_date DATETIME NOT NULL,
  returned_at DATETIME NULL,
  status VARCHAR(50) DEFAULT 'Borrowed',
  FOREIGN KEY (user_id) REFERENCES users(id),
  FOREIGN KEY (book_id) REFERENCES books(id)
);
```

### Reviews Table

```sql
CREATE TABLE reviews (
  id INT PRIMARY KEY AUTO_INCREMENT,
  user_id INT NOT NULL,
  book_id INT NOT NULL,
  rating INT CHECK (rating >= 1 AND rating <= 5),
  comment TEXT,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NULL,
  FOREIGN KEY (user_id) REFERENCES users(id),
  FOREIGN KEY (book_id) REFERENCES books(id)
);
```

### Relationships

```
User ??? Borrowing ??? Book
User ??? Review ??? Book
```

---

## ?? Authentication & Authorization

### JWT Token Structure

The API uses JWT (JSON Web Tokens) for stateless authentication. Tokens contain the following claims:

```json
{
  "sub": "user-id",
  "name": "username",
  "email": "user@example.com",
  "role": "user|admin",
  "jti": "unique-token-id",
  "exp": 1234567890
}
```

### Role-Based Access Control

- **Admin**: Full access to all operations, book management
- **User**: Can borrow/return books, create reviews, view profile

### Using Bearer Token

Include the JWT in the `Authorization` header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Token Extraction Service

Use `IUsrTokenContext` to safely extract user information from tokens:

```csharp
public class MyController : ControllerBase
{
    private readonly IUsrTokenContext _usrTokenContext;

    public MyController(IUsrTokenContext usrTokenContext)
    {
        _usrTokenContext = usrTokenContext;
    }

    public IActionResult MyMethod()
    {
        int userId = _usrTokenContext.GetUserId();
        string username = _usrTokenContext.GetUsername();
        string email = _usrTokenContext.GetEmail();
        string role = _usrTokenContext.GetRole();

        return Ok(new { userId, username, email, role });
    }
}
```

---

## ?? Service Layer

### BookService

Handles book-related operations including creation, updates, and database interactions.

```csharp
public class BookService
{
    public async Task<Book> CreateBook(BookBase book)
    {
        // Create new book with validation
    }
}
```

### BorrowingService

Manages borrowing transactions with ACID compliance:

```csharp
public class BorrowingService
{
    public async Task<(bool, string, BorrowingResponseDto?)> BorrowBookAsync(BorrowRequestDto request)
    {
        // Validates user and book availability
        // Updates copies_available atomically
        // Returns transaction result
    }

    public async Task<(bool, string, BorrowingResponseDto?)> ReturnBookAsync(ReturnRequestDto request)
    {
        // Marks borrowing as returned
        // Restores book copies
        // Returns status
    }
}
```

### ReviewService

Handles review creation, updates, and deletion with ownership validation.

### JwtService

Generates JWT tokens for authenticated users:

```csharp
public class JwtService : IJwtService
{
    public string GenerateToken(User user)
    {
        // Creates JWT with user claims
        // Token expires in 7 days (configurable)
    }
}
```

### UsrTokenContext

Safely extracts user data from JWT claims:

```csharp
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
```

---

## ?? Error Handling

The API uses a consistent error response format:

### Success Response

```json
{
  "message": "Operation successful",
  "data": { /* response data */ }
}
```

### Error Response (400 Bad Request)

```json
{
  "message": "Error description"
}
```

### Error Response (401 Unauthorized)

```json
{
  "message": "Invalid email or password."
}
```

### Error Response (404 Not Found)

```json
{
  "message": "Resource not found"
}
```

### Common Error Scenarios

| Scenario | Status | Message |
|----------|--------|---------|
| Invalid credentials | 401 | "Invalid email or password." |
| Duplicate email | 400 | "Email is already registered." |
| Duplicate username | 400 | "Username is already taken." |
| Book not found | 404 | "Book not found." |
| No copies available | 400 | "No copies available." |
| Already borrowed | 400 | "User already has this book borrowed." |
| User not authenticated | 401 | "Unauthorized" |
| Invalid JWT | 401 | "Invalid token" |

---

## ?? CORS Configuration

The API is configured to accept requests from the Angular frontend running on `http://localhost:4200`.

**To allow additional origins**, update `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "https://yourdomain.com"  // Add your domain
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
```

---

## ?? Request/Response Flow

```
Client Request
     ?
Routing (UseRouting)
     ?
CORS Policy Check (UseCors)
     ?
Authentication (UseAuthentication)
     ?
Authorization (UseAuthorization)
     ?
Controller Action
     ?
Service Layer (Business Logic)
     ?
Database (EF Core/ADO.NET)
     ?
Response to Client
```

---

## ?? Validation

### FluentValidation Integration

The API uses FluentValidation for input validation:

```csharp
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssemblyContaining<BookValidator>());
```

### Validators

- **BookValidator**: Validates book creation/update requests
- **BorrowingValidator**: Validates borrowing requests

---

## ?? Development

### Running in Development Mode

```bash
dotnet run --environment Development
```

### Viewing Logs

Logs are configured in `appsettings.json`. Check the output window for application logs.

### Database Migrations

```bash
# Create a new migration
dotnet ef migrations add <MigrationName>

# Apply migrations
dotnet ef database update

# Revert to previous migration
dotnet ef database update <PreviousMigrationName>
```

### Testing Endpoints

Use **Swagger UI** at `https://localhost:5001/swagger` or tools like:
- Postman
- Thunder Client (VS Code)
- REST Client (VS Code)

---

## ?? Example API Calls

### Register a User

```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "email": "john@example.com",
    "password": "SecurePass123!",
    "firstName": "John",
    "lastName": "Doe",
    "phone": "+1234567890"
  }'
```

### Login

```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePass123!"
  }'
```

### Search Books

```bash
curl -X GET "https://localhost:5001/api/books?search=Harry&page=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Borrow a Book

```bash
curl -X POST https://localhost:5001/api/borrowings/borrow \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "bookId": 5,
    "borrowDays": 14
  }'
```

---

## ?? Contributing

Contributions are welcome! To contribute:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Code Style

- Follow C# naming conventions (PascalCase for public members)
- Use meaningful variable names
- Add XML documentation comments for public methods
- Keep methods focused and concise

---

## ?? License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## ?? Future Enhancements

- [ ] Implement rate limiting
- [ ] Add email notifications for due books
- [ ] Book recommendation system
- [ ] Advanced search filters
- [ ] User activity logging
- [ ] Fine/penalty system
- [ ] Mobile app integration
- [ ] Integration with external book APIs
- [ ] Analytics dashboard
- [ ] Multi-language support

---

## ?? Support

For issues, questions, or suggestions, please:
- Open an [Issue](https://github.com/Mayukh-Ghara/Library_Management/issues)
- Create a [Discussion](https://github.com/Mayukh-Ghara/Library_Management/discussions)
- Contact the maintainers

---

## ????? Project Maintainer

**Mayukh Ghara**  
GitHub: [@Mayukh-Ghara](https://github.com/Mayukh-Ghara)

---

## ?? Acknowledgments

- ASP.NET Core team for the excellent framework
- Entity Framework Core for robust ORM
- FluentValidation for elegant validation
- Swagger/Swashbuckle for API documentation

---

**Last Updated**: 2024  
**Version**: 1.0.0  
**Branch**: `feature/dapper-users`
