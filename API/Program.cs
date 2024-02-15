using API.Data.Persistence;
using API.Endpoints;
using API.Models.Dto.Users;
using API.Repositories;
using API.Repositories.Interfaces;
using API.Security;
using API.Security.Interfaces;
using API.Utils;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddScoped<IPasswordManager, PasswordManager>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<IValidator<CreateUserDto>, CreateUserValidator>();

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
app.MapUserEndpoints();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.Run();
