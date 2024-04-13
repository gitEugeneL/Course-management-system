using API.Data.Entities;
using API.Dto;
using API.Dto.Courses;
using Api.Helpers;
using API.Repositories.Interfaces;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
using API.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using CoursePaginatedRequest = API.Dto.Courses.CoursePaginatedRequest;

namespace API.Endpoints;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this IEndpointRouteBuilder builder)
    {
        var courseGroup = builder.MapGroup("api/v{version:apiVersion}/courses")
            .WithApiVersionSet(ApiVersioning.VersionSet(builder))
            .MapToApiVersion(1)
            .WithTags(nameof(Course));
        
        courseGroup.MapPost("", CreateCourse)
            .RequireAuthorization(AppConstants.ProfessorPolicy)
            .WithValidator<CreateCourseDto>()
            .Produces<CourseResponseDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict);

        courseGroup.MapGet("", GetAllCoursesPaginated)
            .RequireAuthorization(AppConstants.BasePolicy)
            .Produces<PaginatedResponse<CourseResponseDto>>();
        
        courseGroup.MapGet("{courseName}", GetCourseByName)
            .RequireAuthorization(AppConstants.BasePolicy)
            .Produces<CourseResponseDto>()
            .Produces(StatusCodes.Status404NotFound);
        
        courseGroup.MapPut("", UpdateCourse)
            .RequireAuthorization(AppConstants.ProfessorPolicy)
            .WithValidator<UpdateCourseDto>()
            .Produces<CourseResponseDto>()
            .Produces(StatusCodes.Status404NotFound);

        courseGroup.MapDelete("{courseName}", DeleteCourse)
            .RequireAuthorization(AppConstants.ProfessorPolicy)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        courseGroup.MapPatch("join/{courseName}", Join)
            .RequireAuthorization(AppConstants.StudentPolicy)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized);
        
        courseGroup.MapPatch("leave/{courseName}", Leave)
            .RequireAuthorization(AppConstants.StudentPolicy)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);
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
        [AsParameters] CoursePaginatedRequest parameters,
        HttpContext httpContext,
        ICourseRepository repository)
    {
        var (courses, count) = await repository
            .FindAllCoursesPagination(
                parameters.PageNumber, 
                parameters.PageSize, 
                parameters.SortByCreated,
                parameters.SortByAvailableCourses,
                parameters.SortByMyCourses,
                BaseService.ReadUserIdFromToken(httpContext));
        
        return TypedResults.Ok(new PaginatedResponse<CourseResponseDto>(
            courses
                .Select(c => new CourseResponseDto(c))
                .ToList(),
            count,
            parameters.PageNumber,
            parameters.PageSize));
    }
    
    private static async Task<Results<Ok<CourseResponseDto>, NotFound<string>>> GetCourseByName(
        [FromRoute] string courseName,
        ICourseRepository repository)
    {
        var course = await repository.FindCourseByName(courseName);
        return course is not null
            ? TypedResults.Ok(new CourseResponseDto(course))
            : TypedResults.NotFound($"Course {courseName} not found");
    }

    private static async Task<Results<Ok<CourseResponseDto>, NotFound<string>>> UpdateCourse(
        [FromBody] UpdateCourseDto dto,
        ICourseRepository repository)
    {
        var course = await repository.FindCourseById(dto.CourseId);
        if (course is null)
            return TypedResults.NotFound($"Course {dto.CourseId} not found");
        
        course.Description = dto.Description ?? course.Description;
        course.MaxParticipants = dto.MaxParticipant ?? course.MaxParticipants;
        course.Finalized = dto.Finalize ?? course.Finalized;
        
        await repository.UpdateCourse(course);
        return TypedResults.Ok(new CourseResponseDto(course));
    }

    private static async Task<Results<NoContent, NotFound<string>>> DeleteCourse(
        [FromRoute] string courseName,
        ICourseRepository repository)
    {
        var course = await repository.FindCourseByName(courseName);
        if (course is null)
            return TypedResults.NotFound($"Course {courseName} not found");

        await repository.DeleteCourseAndParticipants(course);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NotFound<string>, Conflict<string>, UnauthorizedHttpResult, Ok>> Join(
        [FromRoute] string courseName,
        HttpContext httpContext,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        var course = await courseRepository.FindCourseByName(courseName);
        if (course is null)
            return TypedResults.NotFound($"Course {courseName} not found");

        var currentUserId = BaseService.ReadUserIdFromToken(httpContext);
        if (course.Participants.Any(p => p.UserId == currentUserId))
            return TypedResults.Conflict("You're already in this course");

        if (course.Participants.Count >= course.MaxParticipants || course.Finalized)
            return TypedResults.Conflict("Course is full");
        
        var user = await userRepository.FindUserById(currentUserId);
        if (user is null)
            return TypedResults.Unauthorized();
        
        course.Participants.Add(new Participant { User = user, Course = course });
        await courseRepository.UpdateCourse(course);
        return TypedResults.Ok();
    }
    
    private static async Task<Results<NotFound<string>, UnauthorizedHttpResult, Ok>> Leave(
        [FromRoute] string courseName,
        HttpContext httpContext,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        var course = await courseRepository.FindCourseByName(courseName);
        if (course is null)
            return TypedResults.NotFound($"Course {courseName} not found");
        
        var currentUserId = BaseService.ReadUserIdFromToken(httpContext);
        var participant = course.Participants.SingleOrDefault(p => p.UserId == currentUserId);
        if (participant is null || course.Finalized)
            return TypedResults.NotFound("You're not in this course");

        course.Participants.Remove(participant);
        await courseRepository.UpdateCourse(course);
        return TypedResults.Ok();
    }
}