using System.Security.Claims;
using System.Text;
using API.Data.Enums;
using API.Data.Persistence;
using API.Dto.Auth;
using API.Dto.Courses;
using API.Dto.Participants;
using API.Dto.Users;
using API.Endpoints;
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
    .AddScoped<ICourseRepository, CourseRepository>()
    .AddScoped<IParticipantRepository, ParticipantRepository>()
    .AddScoped<IValidator<LoginDto>, LoginValidator>()
    .AddScoped<IValidator<RefreshDto>, RefreshValidator>()
    .AddScoped<IValidator<CreateUserDto>, CreateUserValidator>()
    .AddScoped<IValidator<CreateCourseDto>, CreateCourseValidator>()
    .AddScoped<IValidator<UpdateCourseDto>, UpdateCourseDtoValidator>()
    .AddScoped<IValidator<GradeParticipantDto>, GradeParticipantDtoValidator>();

/*** Database connection ***/
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));

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

/*** Auth policies ***/
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("professor-policy", policy =>
        policy
            .RequireRole(Role.Professor.ToString())
            .RequireClaim(ClaimTypes.Email)
            .RequireClaim(ClaimTypes.NameIdentifier))
    .AddPolicy("student-policy", policy =>
        policy
            .RequireRole(Role.Student.ToString())
            .RequireClaim(ClaimTypes.Email)
            .RequireClaim(ClaimTypes.NameIdentifier));

/*** Global exception handler ***/
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();


/*** Init develop database data ***/
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetService<AppDbContext>()!;
    DataInitializer.Init(context);
}

app.UseSwagger();
app.UseSwaggerUI();

/*** Add Endpoints ***/
app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapCourseEndpoints();
app.MapParticipantEndpoints();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.Run();

public abstract partial class Program { } // for tests
