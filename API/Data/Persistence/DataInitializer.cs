using API.Data.Entities;
using API.Data.Enums;
using API.Security;

namespace API.Data.Persistence;

public static class DataInitializer
{
    public const string TestProfessorEmail = "professor@test.com";
    public const string TestStudentEmail = "student@test.com";
    public const string TestPassword = "passwordPassword";
    
    public static void Init(AppDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        Seed(context);
    }
    
    private static async void Seed(AppDbContext context)
    {
        var passwordManager = new PasswordManager();
        passwordManager.CreatePasswordHash(TestPassword, out var hash, out var salt);
        
        var student = new User(
            TestStudentEmail, hash, salt, Role.Student, "-", "Olivia", "Johnson");
        var professor = new User(
            TestProfessorEmail, hash, salt, Role.Professor, "40587", "Isabella", "Martinez");

        await context.Users.AddRangeAsync(student, professor);
        
        for (var i = 1; i <= 15; i++)
        {
            var course = new Course(
                $"Course{i}", "some text", new Random().Next(1, 101), professor.Id);
            await context.Courses.AddAsync(course);
        }
        
        await context.SaveChangesAsync();
    }



}