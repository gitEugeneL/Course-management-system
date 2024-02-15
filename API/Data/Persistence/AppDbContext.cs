using System.Reflection;
using API.Data.Entities;
using API.Data.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; init; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
    
    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken token = default)
    {
        foreach (var entity in ChangeTracker.Entries()
                     .Where(x => x is { Entity: BaseAuditableEntity, State: EntityState.Modified })
                     .Select(x => x.Entity)
                     .Cast<BaseAuditableEntity>())
        {
            entity.Updated = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, token);
    }
}