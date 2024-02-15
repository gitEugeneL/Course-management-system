using API.Data.Entities;
using API.Data.Enums;
using API.Models.Dto.Users;
using API.Repositories.Interfaces;
using API.Security.Interfaces;
using API.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder builder)
    {
        var userGroup = builder.MapGroup("api/users")
            .WithTags("Users");

        userGroup.MapPost("", CreateUser)
            .WithValidator<CreateUserDto>()
            .Produces<UserResponseDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict);

        userGroup.MapGet("", () => TypedResults.Ok());
    }
    
    private static async Task<Results<Created<UserResponseDto>, Conflict<string>>> CreateUser(
        [FromBody] CreateUserDto dto,
        IPasswordManager pwdManager,
        IUserRepository repository)
    {
        if (await repository.UserExists(dto.Email))
            return TypedResults.Conflict($"User: {dto.Email} already exists");

        pwdManager.CreatePasswordHash(dto.Password, out var hash, out var salt);
        var user = new User(dto.Email, hash, salt, Role.Student, dto.UniversityNumber, dto.FirstName, dto.LastName);

        await repository.CreateUser(user);
        return TypedResults.Created(user.Id.ToString(), new UserResponseDto(user));
    }
}