namespace API.Dto.Courses;

public sealed record CoursePaginatedRequest(
    int PageNumber = 1,
    int PageSize = 10,
    bool SortByCreated = false,
    bool SortByAvailableCourses = false,
    bool SortByMyCourses = false
);
