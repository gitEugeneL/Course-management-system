using System.Net;
using System.Security.Cryptography;
using API.Data.Persistence;
using API.Dto.Auth;
using API.Dto.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace API.IntegrationTests.Endpoints;

public class AuthEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = TestCase.CreateTestHttpClient(factory, "authTestsDb");

    [Theory]
    [InlineData("test@email.com", "strongPwd!", "John", "Smith", "4568")]
    [InlineData("test2@email.com", "myPassword", "Emily", "Johnson", "AD123")]
    public async Task Register_withValidBody_ReturnsCreatedResult(
        string email, string password, string firstName, string lastName, string universityNumber)
    {
        // arrange
        var model = new CreateUserDto(email, password, firstName, lastName, universityNumber);

        // act
        var response = await _client.PostAsync("api/auth/register", TestCase.CreateContext(model));
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task Register_withExistingUser_ReturnsConflictResult()
    {
        // arrange
        var model = new CreateUserDto(
            DataInitializer.TestStudentEmail, "stringPwd", "test", "test", "4544");
        
        // act
        var response = await _client.PostAsync("api/auth/register", TestCase.CreateContext(model));
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task Login_withValidUser_ReturnsOkResult()
    {
        // arrange
        var model = new LoginDto(DataInitializer.TestStudentEmail, DataInitializer.TestPassword);
        
        // act
        var response = await _client.PostAsync("api/auth/login", TestCase.CreateContext(model));
        
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
        var response = await _client.PostAsync("api/auth/login", TestCase.CreateContext(model));
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Refresh_withValidRefreshToken_ReturnsOkResult()
    {
        // arrange
        var loginResponse = await TestCase.Login(_client, DataInitializer.TestStudentEmail, DataInitializer.TestPassword);
        var model = new RefreshDto(loginResponse.RefreshToken);
        
        // act
        var response = await _client.PostAsync("api/auth/refresh", TestCase.CreateContext(model));
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Refresh_withInvalidRefreshToken_ReturnsUnauthorizedResult()
    {
        // arrange
        var model = new RefreshDto(Convert.ToBase64String(RandomNumberGenerator.GetBytes(256)));
        
        // act
        var response = await _client.PostAsync("api/auth/refresh", TestCase.CreateContext(model));
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Logout_withValidRefreshToken_ReturnsNoContentResult()
    {
        // arrange
        var loginResponse = await TestCase.Login(_client, DataInitializer.TestStudentEmail, DataInitializer.TestPassword);
        var model = new RefreshDto(loginResponse.RefreshToken);
        
        // act
        var response = await _client.PostAsync("api/auth/logout", TestCase.CreateContext(model));
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task Logout_withInvalidRefreshToken_ReturnsUnauthorizedResult()
    {
        // arrange
        var model = new RefreshDto(Convert.ToBase64String(RandomNumberGenerator.GetBytes(256)));
        
        // act
        var response = await _client.PostAsync("api/auth/logout", TestCase.CreateContext(model));
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}