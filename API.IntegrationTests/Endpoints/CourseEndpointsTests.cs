using System.Net;
using API.Data.Persistence;
using API.Dto;
using API.Dto.Courses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace API.IntegrationTests.Endpoints;

public class CourseEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = TestCase.CreateTestHttpClient(factory, "courseTestsDb");
    
    private async Task<HttpResponseMessage> CreateTestCourse()
    {
        var model = new CreateCourseDto("Math", "Math course", 10);
        return await _client.PostAsJsonAsync($"api/v{TestCase.ApiVersion}/courses", model);
    }
    
    [Theory]
    [InlineData("Math", "Math course", 35)]
    [InlineData("Biology", "History course", 10)]
    public async Task CreateCourse_withValidBody_ReturnsCreatedResult(
        string name,
        string description,
        int maxParticipants)
    {
        // arrange
        var model = new CreateCourseDto(name, description, maxParticipants);
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        
        // act
        var response = await _client.PostAsJsonAsync($"api/v{TestCase.ApiVersion}/courses", model);
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task CreateCourse_withExistingCourse_ReturnsConflictResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        await CreateTestCourse();
        
        // act
        var response = await CreateTestCourse();
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task GetCourseByName_withValidName_ReturnsOkResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        var createResponse = await CreateTestCourse();
        var course = await TestCase.DeserializeResponse<CourseResponseDto>(createResponse);
        
        // act
        var response = await _client.GetAsync($"api/v{TestCase.ApiVersion}/courses/{course!.Name}");
      
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var courseResponseDto = await TestCase.DeserializeResponse<CourseResponseDto>(response);
        courseResponseDto.Should().NotBeNull();
        courseResponseDto!.Name.Should().Be(course.Name);
    }
    
    [Fact]
    public async Task GetCourseByName_withInvalidName_ReturnsNotFoundResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
    
        // act
        var response = await _client.GetAsync($"api/v{TestCase.ApiVersion}/courses/invalid-course-name");
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 5)]
    public async Task GetAllGetAllCoursesPaginated_withValidRequest_ReturnsOkResult(int pageNumber, int pageSize)
    {
        // arrange
        var queryParams = $"?pageNumber={pageNumber}&pageSize={pageSize}";
        await TestCase.Login(_client, DataInitializer.TestStudentEmail, DataInitializer.TestPassword);
        
        // act
        var response = await _client.GetAsync($"api/v{TestCase.ApiVersion}/courses{queryParams}");
        
        // assert 
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var courseResponses = await TestCase.DeserializeResponse<PaginatedResponse<CourseResponseDto>>(response);
        courseResponses!.PageNumber.Should().Be(pageNumber);
        courseResponses.Items.Count.Should().Be(pageSize);
    }
    
    [Fact]
    public async Task UpdateCourse_WithValidData_ReturnsOkResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        var createResponse = await CreateTestCourse();
        var course = await TestCase.DeserializeResponse<CourseResponseDto>(createResponse);
        var updateModel = new UpdateCourseDto(course!.CourseId, null, null, true);
        
        // act
        var response = await _client.PutAsJsonAsync($"api/v{TestCase.ApiVersion}/courses", updateModel);
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCourse = await TestCase.DeserializeResponse<CourseResponseDto>(response);
        updatedCourse!.CourseId.Should().Be(course.CourseId);
        updatedCourse.Finalized.Should().Be((bool)updateModel.Finalize!);
    }
    
    [Fact]
    public async Task UpdateCourse_WithInvalidData_ReturnsNotFoundResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        var updateModel = new UpdateCourseDto(Guid.NewGuid(), null, 50, null);
        
        // act
        var response = await _client.PutAsJsonAsync($"api/v{TestCase.ApiVersion}/courses", updateModel);
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteCourse_WithValidCourse_ReturnsNoContextResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        var createResponse = await CreateTestCourse();
        var course = await TestCase.DeserializeResponse<CourseResponseDto>(createResponse);
        
        // act
        var response = await _client.DeleteAsync($"api/v{TestCase.ApiVersion}/courses/{course!.Name}");
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task DeleteCourse_WithInvalidCourse_ReturnsNoFoundResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        
        // act
        var response = await _client.DeleteAsync($"api/v{TestCase.ApiVersion}/courses/invalid-course-name");
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Join_WithValidCourseAndUser_ReturnsOkResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        var createResponse = await CreateTestCourse();
        var course = await TestCase.DeserializeResponse<CourseResponseDto>(createResponse);
        await TestCase.Login(_client, DataInitializer.TestStudentEmail, DataInitializer.TestPassword);
        
        // act
        var response = await _client.PatchAsync($"api/v{TestCase.ApiVersion}/courses/join/{course!.Name}", null);
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Join_WithValidCourseAndInvalidUser_ReturnsConflictResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        var createResponse = await CreateTestCourse();
        var course = await TestCase.DeserializeResponse<CourseResponseDto>(createResponse);
        await TestCase.Login(_client, DataInitializer.TestStudentEmail, DataInitializer.TestPassword);
        
        // act
        var response = new HttpResponseMessage();
        for (var i = 0; i < 2; i++)
            response = await _client.PatchAsync($"api/v{TestCase.ApiVersion}/courses/join/{course!.Name}", null);
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task Join_WithInvalidCourseAndValidUser_ReturnsNotFoundResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestStudentEmail, DataInitializer.TestPassword);
        
        // act
        var response = await _client.PatchAsync($"api/v{TestCase.ApiVersion}/courses/join/invalid-course-name", null);
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Leave_WithValidCourseAndUser_ReturnsOkResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        var createResponse = await CreateTestCourse();
        var course = await TestCase.DeserializeResponse<CourseResponseDto>(createResponse);
        await TestCase.Login(_client, DataInitializer.TestStudentEmail, DataInitializer.TestPassword);
        await _client.PatchAsync($"api/v{TestCase.ApiVersion}/courses/join/{course!.Name}", null);
        
        // act
        var response = await _client.PatchAsync($"api/v{TestCase.ApiVersion}/courses/leave/{course.Name}", null);
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Leave_WithInvalidCourseAndValidUser_ReturnsNotFoundResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestStudentEmail, DataInitializer.TestPassword);
        
        // act
        var response = await _client.PatchAsync($"api/v{TestCase.ApiVersion}/courses/leave/invalid-course-name", null);
    
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Leave_WithValidCourseAndInvalidUser_ReturnsNotFoundResult()
    {
        // arrange
        await TestCase.Login(_client, DataInitializer.TestProfessorEmail, DataInitializer.TestPassword);
        var createResponse = await CreateTestCourse();
        var course = await TestCase.DeserializeResponse<CourseResponseDto>(createResponse);
        await TestCase.Login(_client, DataInitializer.TestStudentEmail, DataInitializer.TestPassword);
        
        // act
        var response = await _client.PatchAsync($"api/v{TestCase.ApiVersion}/courses/leave/{course!.Name}", null);
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
