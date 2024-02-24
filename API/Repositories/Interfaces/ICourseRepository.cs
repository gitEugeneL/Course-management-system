using API.Data.Entities;

namespace API.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<bool> CreateCourse(Course course);

    Task<bool> UpdateCourse(Course course);
    
    Task<bool> CourseExist(string name);

    Task<Course?> FindCourseByName(string name);

    Task<Course?> FindCourseById(Guid id);

    Task<(IEnumerable<Course> List, int Count)> FindAllCoursesPagination(
        int pageNumber, 
        int pageSize, 
        bool sortByCreated = false, 
        bool sortByAvailableCourses = false,
        bool sortByMyCourses = false,
        Guid? userId = null);

    Task<bool> DeleteCourseAndParticipants(Course course);
}
