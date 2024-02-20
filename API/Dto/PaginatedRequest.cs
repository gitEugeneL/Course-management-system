namespace API.Dto;

public sealed record PaginatedRequest(
    int PageNumber = 1,
    int PageSize = 10,
    bool SortByCreated = false
);
