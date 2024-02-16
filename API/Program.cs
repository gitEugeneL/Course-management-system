using System.Text;
using API.Data.Persistence;
using API.Endpoints;
using API.Models.Dto.Auth;
using API.Models.Dto.Users;
using API.Repositories;
using API.Repositories.Interfaces;
using API.Security;
using API.Security.Interfaces;
using API.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddScoped<IPasswordManager, PasswordManager>()
    .AddScoped<ITokenManager, TokenManager>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<IValidator<LoginDto>, LoginValidator>()
    .AddScoped<IValidator<RefreshDto>, RefreshValidator>()
    .AddScoped<IValidator<CreateUserDto>, CreateUserValidator>();

/*** JWT  auth configuration ***/
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("Authentication:Key").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1) // allowed time deviation, 5min - default
        };
    });

/*** Swagger configuration ***/
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JWT Bearer Authorization with refresh token. Example: Bearer {your access token....}",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

/*** Global exception handler ***/
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

/*** Database connection ***/
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

/*** Add Endpoints ***/
app.MapAuthEndpoints();
app.MapUserEndpoints();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.Run();

public abstract partial class Program { } // for tests
