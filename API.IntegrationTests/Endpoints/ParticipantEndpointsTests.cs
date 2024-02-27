using System.Net;
using API.Data.Persistence;
using API.Dto;
using API.Dto.Courses;
using API.Dto.Participants;
using API.Dto.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace API.IntegrationTests.Endpoints;

public class ParticipantEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = TestCase.CreateTestHttpClient(factory, "participantTestsDb");
    
    private async Task<CourseResponseDto?> CreateCourse(string name, int countUsers = 5)
    {
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        var courseModel = new CreateCourseDto(name, "some text", countUsers);
        var courseResponse = await _client.PostAsync("api/courses", TestCase.CreateContext(courseModel));
        return await TestCase.DeserializeResponse<CourseResponseDto>(courseResponse);
    }

    private async Task<UserResponseDto?> CreateUser(string email, string password)
    {
        var userModel = new CreateUserDto(email, password, "user", "user", "-");
        var userResponse = await _client.PostAsync("api/auth/register", TestCase.CreateContext(userModel));
        return await TestCase.DeserializeResponse<UserResponseDto>(userResponse);
    }
    
    [Theory]
    [InlineData(5, "")]
    [InlineData(3, "some text")]
    public async Task Grade_withValidUserAndCourse_ReturnsOkResult(int grade, string note)
    {
        // arrange
        // create user
        const string email = "test@test.com";
        const string password = "password";
        var user = await CreateUser(email, password);
        // create course
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        var course = await CreateCourse("testCourse");
        // join user to the course
        await TestCase.Login(_client, email, password);
        await _client.PatchAsync($"api/courses/join/{course!.Name}", null);
        // grade model
        var gradeModel = new GradeParticipantDto(user!.UserId, course.CourseId, grade, note);
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        
        // act
        var response = await _client.PatchAsync("api/participants", TestCase.CreateContext(gradeModel));
        var updatedParticipant = await TestCase.DeserializeResponse<ParticipantResponseDto>(response);
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedParticipant!.User.Email.Should().Be(user.Email);
        updatedParticipant.Course.Name.Should().Be(course.Name);
        updatedParticipant.Grade.Should().Be(grade);
        updatedParticipant.ProfessorNote.Should().Be(note);
    }
    
    [Fact]    
    public async Task Grade_withValidUserAndInvalidCourse_ReturnsNotFoundResult()
    {
        // arrange
        var user = await CreateUser("test@test.com", "password");
        
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        var gradeModel = new GradeParticipantDto(user!.UserId, Guid.NewGuid(), 5, "some text");

        // act
        var response = await _client.PatchAsync("api/participants", TestCase.CreateContext(gradeModel));
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Theory]
    [InlineData(1, 10, 20)]
    [InlineData(3, 5, 5)]
    public async Task GetAllByCourseName_withValidCourseName_ReturnsOkResult(int pageNumber, int pageSize, int countUsers)
    {
        // arrange
        // create course
        var course = await CreateCourse("testCourse", countUsers);
        
        for (var i = 1; i <= countUsers; i++)
        {
            var email = $"test{i}@test.com";
            const string password = "password";
            await CreateUser(email, password);
            // join user to the course
            await TestCase.Login(_client, email, password);
            await _client.PatchAsync($"api/courses/join/{course!.Name}", null);
        }
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        
        // act
        var queryParams = $"?pageNumber={pageNumber}&pageSize={pageSize}";
        var response = await _client.GetAsync($"api/participants/{course!.Name}{queryParams}");
        var data = await TestCase.DeserializeResponse<PaginatedResponse<ParticipantResponseDto>>(response);
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        data!.PageNumber.Should().Be(pageNumber);
        data.TotalItemsCount.Should().Be(countUsers);
    }

    [Fact]
    public async Task GetAllByCourseName_withInvalidCourseName_ReturnsNotFoundResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        
        // act
        var response = await _client.GetAsync($"api/participants/invalid-course-name");
        
        // asser
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Theory]
    [InlineData(10)]
    [InlineData(5)]
    public async Task GetAllParticipantsByUser_withValidRequest_ReturnsOkResult(int courseCount)
    {
        // arrange
        const string email = "test@test.com";
        const string password = "password";
        await CreateUser(email, password);
        
        for (var i = 1; i <= courseCount; i++)
        {
            var course = await CreateCourse($"testCourse{i}");
            // join user to the course
            await TestCase.Login(_client, email, password);
            await _client.PatchAsync($"api/courses/join/{course!.Name}", null);
        }
        
        // act
        var response = await _client.GetAsync("api/participants");

        // assert
        var data = await TestCase.DeserializeResponse<List<ParticipantResponseDto>>(response);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        data!.Count.Should().Be(courseCount);
    }
}
