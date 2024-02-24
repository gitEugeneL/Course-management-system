using API.Data.Entities;
using API.Data.Persistence;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

internal class CourseRepository(AppDbContext context) : ICourseRepository
{
    public async Task<bool> CreateCourse(Course course)
    {
        await context
            .Courses
            .AddAsync(course);
        return await context
            .SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateCourse(Course course)
    {
        context.Courses.Update(course);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> CourseExist(string name)
    {
        return await context
            .Courses
            .AnyAsync(c => c.Name.ToLower()
                .Equals(name.ToLower()));
    }

    public async Task<Course?> FindCourseByName(string name)
    {
        return await context
            .Courses
            .Include(c => c.Participants)
            .SingleOrDefaultAsync(c => c.Name.ToLower()
                .Equals(name.ToLower()));
    }

    public async Task<Course?> FindCourseById(Guid id)
    {
        return await context
            .Courses
            .Include(c => c.Participants)
            .SingleOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<(IEnumerable<Course> List, int Count)> FindAllCoursesPagination(
        int pageNumber, 
        int pageSize, 
        bool sortByCreated = false,
        bool sortByAvailableCourses  = false,
        bool sortByMyCourses = false,
        Guid? userId = null)
    {
        var query = context
            .Courses
            .Include(c => c.Participants)
            .AsQueryable();

        if (sortByCreated)
            query = query.OrderBy(c => c.Created);

        if (sortByMyCourses)
            query = query
                .Where(c => c.Participants.Any(p => p.UserId == userId));
        
        if (sortByAvailableCourses)
            query = query
                .Where(c => c.Participants.Count < c.MaxParticipants);
        
        var count = await query
            .CountAsync();

        var courses = await query
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (courses, count);
    }

    public async Task<bool> DeleteCourseAndParticipants(Course course)
    {
        var participantsToDelete = context
            .Participants
            .Where(p => p.CourseId == course.Id);
        
        context
            .Participants
            .RemoveRange(participantsToDelete);

        context
            .Courses
            .Remove(course);

        return await context.SaveChangesAsync() > 0;
    }
}