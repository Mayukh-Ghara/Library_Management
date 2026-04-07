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
        var jwtKey = builder.Configuration["Jwt:Key"];

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

        // ADO.NET (if needed)
        builder.Services.AddSingleton<UserDbContext>();

        // Services
        builder.Services.AddScoped<BookService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddTransient<IJwtService, JwtService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}