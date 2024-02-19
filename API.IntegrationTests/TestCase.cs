using System.Text;
using API.Data.Persistence;
using API.Models.Dto.Auth;
using API.Models.Dto.Users;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.IntegrationTests;

public class TestCase
{
    protected readonly HttpClient Client;

    protected TestCase(WebApplicationFactory<Program> factory)
    {
        Client = factory
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

    protected readonly CreateUserDto User = new(
        "student@test.con", 
        "strongPwd", 
        "Student1", 
        "Student1", 
        "45605");

    protected static StringContent CreateContext(object o)
    {
        return new StringContent(
            JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
    }
    
    protected async Task<HttpResponseMessage> CreateUser(CreateUserDto dto)
    {
        var context = CreateContext(dto);
        return await Client.PostAsync("api/auth/register", context);
    }

    protected async Task<LoginResponseDto> Login(string email, string password)
    {
        var model = new LoginDto(email, password);
        var response = await Client.PostAsync("api/auth/login", CreateContext(model));
        return JsonConvert.DeserializeObject<LoginResponseDto>(await response.Content.ReadAsStringAsync())!;
    }
}