using API.Data.Entities;
using API.Dto;
using API.Dto.Courses;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using API.Utils;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Endpoints;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this IEndpointRouteBuilder builder)
    {
        var courseGroup = builder.MapGroup("api/courses")
            .WithTags("Courses");
    
        courseGroup.MapPost("", CreateCourse)
            .RequireAuthorization("professor-policy")
            .WithValidator<CreateCourseDto>()
            .Produces<CourseResponseDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict);

        courseGroup.MapGet("", GetAllCoursesPaginated)
            .RequireAuthorization("student-policy")
            .Produces<PaginatedResponse<CourseResponseDto>>();
        
        courseGroup.MapGet("{courseName}", GetCourseByName)
            .RequireAuthorization("student-policy")
            .Produces<CourseResponseDto>()
            .Produces(StatusCodes.Status404NotFound);

        // todo update
        
        // todo get all participants by course name
        
        // todo get available courses 
        
        // todo delete course
        
        // todo get my courses
        
        // todo join / leave
    }
    
    private static async Task<Results<Created<CourseResponseDto>, Conflict<string>>> CreateCourse(
        [FromBody] CreateCourseDto dto,
        ICourseRepository repository,
        HttpContext httpContext)
    {
        if (await repository.CourseExist(dto.Name))
            return TypedResults.Conflict($"Course: {dto.Name} already exists");

        var currentUserId = BaseService.ReadUserIdFromToken(httpContext);
        var course = new Course(dto.Name, dto.Description, dto.MaxParticipants, currentUserId);
            
        await repository.CreateCourse(course);
        return TypedResults.Created(course.Id.ToString(), new CourseResponseDto(course));
    }

    private static async Task<IResult> GetAllCoursesPaginated(
        [AsParameters] PaginatedRequest parameters,
        ICourseRepository repository)
    {
        var (courses, count) = await repository
            .GetAllCoursesPagination(
                parameters.PageNumber, 
                parameters.PageSize, 
                parameters.SortByCreated);
        
        return TypedResults.Ok(new PaginatedResponse<CourseResponseDto>(
            courses
                .Select(c => new CourseResponseDto(c))
                .ToList(),
            count,
            parameters.PageNumber,
            parameters.PageSize));
    }
    
    private static async Task<IResult> GetCourseByName(
        [FromRoute] string courseName,
        ICourseRepository repository)
    {
        var course = await repository.GetCourseByName(courseName);
        return course is not null
            ? TypedResults.Ok(new CourseResponseDto(course))
            : TypedResults.NotFound($"Course {courseName} not found");
    }
}