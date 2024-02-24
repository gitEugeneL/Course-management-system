using API.Data.Entities;
using API.Data.Persistence;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class ParticipantRepository(AppDbContext context) : IParticipantRepository
{
    public async Task<Participant?> FindParticipantByUserIdAndCourseId(Guid userId, Guid courseId)
    {
        return await context
            .Participants
            .Include(p => p.User)
            .Include(p => p.Course)
            .SingleOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId);
    }

    public async Task<bool> UpdateParticipant(Participant participant)
    {
        context
            .Participants
            .Update(participant);
        return await context.SaveChangesAsync() > 0;

    }

    public async Task<IEnumerable<Participant>> FindParticipantsByUserId(Guid userId)
    {
        return await context
            .Participants
            .Include(p => p.User)
            .Include(p => p.Course)
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Participant> List, int Count)> FindAllByCoursePagination(
        int pageNumber, 
        int pageSize, 
        Course course)
    {
        var query = context
            .Participants
            .Include(p => p.Course)
            .Include(p => p.User)
            .Where(p => p.Course == course)
            .AsQueryable();
        
        var count = await query
            .CountAsync();

        var participants = await query
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (participants, count);
    }
}