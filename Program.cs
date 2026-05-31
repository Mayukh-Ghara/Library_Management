using System.Text;
using FluentValidation.AspNetCore;
using LibraryWebAPI.Data;
using LibraryWebAPI.Services;
using LibraryWebAPI.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Load configuration values
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        // CORRECT: Matching exactly what JwtService uses!
        var jwtKey = builder.Configuration["JwtSettings:SecretKey"];

        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("Database connection string is missing.");

        if (string.IsNullOrEmpty(jwtKey))
            throw new Exception("JWT Key is missing.");

        // JWT Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

        builder.Services.AddAuthorization();

        // Controllers + FluentValidation
        builder.Services.AddControllers()
            .AddFluentValidation(fv =>
                fv.RegisterValidatorsFromAssemblyContaining<BookValidator>());

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // EF Core (MySQL)
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            ));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularApp", policy =>
            {
                policy.WithOrigins("http://localhost:4200") // Trust your Angular dev server
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // ADO.NET (if needed)
        builder.Services.AddSingleton<UserDbContext>();

        // HTTP Context for token extraction
        builder.Services.AddHttpContextAccessor();

        // Services
        builder.Services.AddScoped<BookService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddTransient<IJwtService, JwtService>();
        builder.Services.AddScoped<ReviewService>();
        builder.Services.AddScoped<BorrowingService>();
        builder.Services.AddScoped<IUsrTokenContext, UsrTokenContext>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // 1. First, figure out where the request is going
        app.UseRouting();

        // 2. Next, apply the CORS policy so Angular is allowed to talk to it
        app.UseCors("AllowAngularApp");

        // 3. Then identify WHO the user is
        app.UseAuthentication();

        // 4. Finally, check WHAT they are allowed to do (Authorization)
        app.UseAuthorization();

        // 5. Map the request to the correct Controller
        app.MapControllers();

        app.Run();
    }
}