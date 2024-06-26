using System.Net.Http.Headers;
using API.Data.Persistence;
using API.Dto.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.IntegrationTests;

public static class TestCase
{
    public const int ApiVersion = 1;
    
    public static HttpClient CreateTestHttpClient(WebApplicationFactory<Program> factory, string dbname)
    {
        return factory
            .WithWebHostBuilder(builder => 
            {
                builder.ConfigureServices(services =>
                {
                    services.Remove(services.SingleOrDefault(service =>
                        service.ServiceType == typeof(DbContextOptions<AppDbContext>))!);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase(dbname));
                });
            })
            .CreateClient();
    }
    
    public static async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var jsonResponse = await response.Content.ReadAsStringAsync(); 
        return JsonConvert.DeserializeObject<T>(jsonResponse);
    }
    
    public static async Task<LoginResponseDto> Login(HttpClient client, string email, string password)
    {
        var model = new LoginDto(email, password);
        var response = await client.PostAsJsonAsync($"api/v{ApiVersion}/auth/login", model);
        var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(await response.Content.ReadAsStringAsync())!;

        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
        return loginResponse;
    }
}