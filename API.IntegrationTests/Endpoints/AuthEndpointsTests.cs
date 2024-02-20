using System.Net;
using System.Security.Cryptography;
using API.Dto.Auth;
using API.Dto.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace API.IntegrationTests.Endpoints;

public class AuthEndpointsTests(WebApplicationFactory<Program> factory)
    : TestCase(factory), IClassFixture<WebApplicationFactory<Program>>
{
    [Theory]
    [InlineData("test@email.com", "strongPwd!", "John", "Smith", "4568")]
    [InlineData("test2@email.com", "myPassword", "Emily", "Johnson", "AD123")]
    public async Task Register_withValidBody_ReturnsCreatedResult(
        string email, string password, string firstName, string lastName, string universityNumber)
    {
        // arrange
        var model = new CreateUserDto(email, password, firstName, lastName, universityNumber);
        // act
        var response = await Client.PostAsync("api/auth/register", CreateContext(model));
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_withExistingUser_ReturnsConflictResult()
    {
        // arrange
        await CreateUser(User);
        // act
        var response = await Client.PostAsync("api/auth/register", CreateContext(User));
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task Login_withValidUser_ReturnsOkResult()
    {
        // arrange
        await CreateUser(User);
        var model = new LoginDto(User.Email, User.Password);
        // act
        var response = await Client.PostAsync("api/auth/login", CreateContext(model));
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("test@email.com", "strongPwd!")]
    [InlineData("test2@email.com", "myPassword")]
    public async Task Login_withNonExistentUser_ReturnsNotFoundResult(string email, string password)
    {
        // arrange
        var model = new LoginDto(email, password);
        // act
        var response = await Client.PostAsync("api/auth/login", CreateContext(model));
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Refresh_withValidRefreshToken_ReturnsOkResult()
    {
        // arrange
        await CreateUser(User);
        var loginResponse = await Login(User.Email, User.Password);
        var model = new RefreshDto(loginResponse.RefreshToken);
        // act
        var response = await Client.PostAsync("api/auth/refresh", CreateContext(model));
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Refresh_withInvalidRefreshToken_ReturnsUnauthorizedResult()
    {
        // arrange
        await CreateUser(User);
        var model = new RefreshDto(Convert.ToBase64String(RandomNumberGenerator.GetBytes(256)));
        // act
        var response = await Client.PostAsync("api/auth/refresh", CreateContext(model));
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Logout_withValidRefreshToken_ReturnsNoContentResult()
    {
        // arrange
        await CreateUser(User);
        var loginResponse = await Login(User.Email, User.Password);
        var model = new RefreshDto(loginResponse.RefreshToken);
        // act
        var response = await Client.PostAsync("api/auth/logout", CreateContext(model));
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Logout_withInvalidRefreshToken_ReturnsUnauthorizedResult()
    {
        // arrange
        await CreateUser(User);
        var model = new RefreshDto(Convert.ToBase64String(RandomNumberGenerator.GetBytes(256)));
        // act
        var response = await Client.PostAsync("api/auth/logout", CreateContext(model));
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}