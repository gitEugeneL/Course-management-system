using API.Data.Entities;
using API.Dto;
using API.Dto.Participants;
using Api.Helpers;
using API.Repositories.Interfaces;
using Api.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using API.Utils;

namespace API.Endpoints;

public static class ParticipantEndpoints
{
    public static void MapParticipantEndpoints(this IEndpointRouteBuilder builder)
    {
        var participantGroup = builder.MapGroup("api/v{version:apiVersion}/participants")
            .WithApiVersionSet(ApiVersioning.VersionSet(builder))
            .MapToApiVersion(1)
            .WithTags(nameof(Participant));
        
        participantGroup.MapPatch("", Grade)
            .RequireAuthorization(AppConstants.ProfessorPolicy)
            .WithValidator<GradeParticipantDto>()
            .Produces<ParticipantResponseDto>()
            .Produces(StatusCodes.Status404NotFound);

        participantGroup.MapGet("", GetAllParticipantsByUser)
            .RequireAuthorization(AppConstants.StudentPolicy)
            .Produces<List<ParticipantResponseDto>>();
        
        participantGroup.MapGet("{courseName}", GetAllByCourseName)
            .RequireAuthorization(AppConstants.BasePolicy)
            .Produces<PaginatedResponse<ParticipantResponseDto>>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<ParticipantResponseDto>, NotFound<string>>> Grade(
        [FromBody] GradeParticipantDto dto,
        IParticipantRepository repository)
    {
        var participant = await repository.FindParticipantByUserIdAndCourseId(dto.UserId, dto.CourseId);
        if (participant is null)
            return TypedResults.NotFound("Participant doesn't exist");

        participant.Grade = dto.Grade;
        participant.ProfessorNote = dto.ProfessorNote ?? participant.ProfessorNote;
        await repository.UpdateParticipant(participant);

        return TypedResults.Ok(new ParticipantResponseDto(participant));
    }

    private static async Task<IResult> GetAllParticipantsByUser(
        HttpContext httpContext, 
        IParticipantRepository repository)
    {
        var currentUserId = BaseService.ReadUserIdFromToken(httpContext);
        var participants = await repository.FindParticipantsByUserId(currentUserId);

        return TypedResults.Ok(
            participants
                .Select(p => new ParticipantResponseDto(p))
                .ToList());
    }
    
    private static async Task<Results<Ok<PaginatedResponse<ParticipantResponseDto>>, NotFound<string>>> 
        GetAllByCourseName(
            [FromRoute] string courseName, 
            [AsParameters] ParticipantPaginatedRequest parameters, 
            IParticipantRepository participantRepository, 
            ICourseRepository courseRepository)
    {
        var course = await courseRepository.FindCourseByName(courseName);
        if (course is null)
            return TypedResults.NotFound($"Course {courseName} not found");
        
        var (participants, count) = await participantRepository
            .FindAllByCoursePagination(parameters.PageNumber, parameters.PageSize, course);

        return TypedResults.Ok(new PaginatedResponse<ParticipantResponseDto>(
            participants
                .Select(p => new ParticipantResponseDto(p))
                .ToList(),
            count,
            parameters.PageNumber,
            parameters.PageSize));
    }
}