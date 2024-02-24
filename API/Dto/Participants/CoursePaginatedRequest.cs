namespace API.Dto.Participants;

public sealed record ParticipantPaginatedRequest(
    int PageNumber = 1,
    int PageSize = 20
);
