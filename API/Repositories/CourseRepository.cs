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

    public async Task<bool> CourseExist(string name)
    {
        return await context
            .Courses
            .AnyAsync(c => c.Name.ToLower()
                .Equals(name.ToLower()));
    }

    public async Task<Course?> GetCourseByName(string name)
    {
        return await context
            .Courses
            .Include(c => c.Participants)
            .SingleOrDefaultAsync(c => c.Name.ToLower()
                .Equals(name.ToLower()));
    }

    public async Task<(IEnumerable<Course> List, int Count)> GetAllCoursesPagination(
        int pageNumber, 
        int pageSize, 
        bool sortByCreated)
    {
        var query = context
            .Courses
            .Include(c => c.Participants)
            .AsQueryable();

        if (sortByCreated)
            query = query.OrderBy(c => c.Created);

        var count = await query
            .CountAsync();

        var courses = await query
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (courses, count);
    }
}