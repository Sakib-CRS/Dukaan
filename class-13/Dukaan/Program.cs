using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Dukaan.Infrastructure.Services;
using Dukaan.Infrastructure.Data.Model;
using Dukaan.Infrastructure.Data.DbContext;
using Dukaan.Infrastructure.Data.Repositories;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Service Registration Section ---
// This is where we register dependencies for the built-in Dependency Injection (DI) container.

// Register the Database Context with PostgreSQL support
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Register ASP.NET Core Identity for authentication
builder.Services.AddIdentity<Merchant, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


var jwtSettings = builder.Configuration.GetSection("Jwt");
// Register AuthenticationSchema for authentication
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });

// Register Authorization
builder.Services.AddAuthorization();

// Register application-specific services and repositories
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped(typeof(Repository<>)); // Registers the generic repository
builder.Services.AddScoped<AuthService>();

// Register OpenAPI (Swagger) for API documentation
builder.Services.AddOpenApi();

// Register MVC controllers
builder.Services.AddControllers();

var app = builder.Build();

// --- 2. Middleware Pipeline Section ---
// This defines the order in which HTTP requests are processed.

if (app.Environment.IsDevelopment())
{
    // Enables the interactive Swagger UI in development mode
    app.MapOpenApi();
}

// Redirects HTTP requests to HTTPS
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Maps controller routes (e.g., [Route("api/[controller]")])
app.MapControllers();

// Starts the application
app.Run();