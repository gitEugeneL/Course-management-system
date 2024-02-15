using System.Net;
using System.Text;
using API.Data.Persistence;
using API.Models.Dto.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace API.IntegrationTests;

public class UserEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public UserEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Remove(services.SingleOrDefault(service =>
                        service.ServiceType == typeof(DbContextOptions<AppDbContext>))!);
                    
                    services.AddDbContext<AppDbContext>(options => 
                        options.UseInMemoryDatabase("InMemoryDb"));
                });
            })
            .CreateClient();
    }
    
    [Theory]
    [InlineData("email1@email.com", "123456789", "firstName1", "lastName1", "404568")]
    [InlineData("email2@email.com", "987654321", "firstName2", "lastName2", "404569")]
    public async Task CreateUser_withValidBody_ReturnsCreatedResul(
        string email, string password, string firstName, string lastName, string universityNumber)
    {
        // arrange
        var model = new CreateUserDto(email, password, firstName, lastName, universityNumber);
        var httpContent = new StringContent(
            JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        
        // act
        var response = await _client.PostAsync("/api/users", httpContent);
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}