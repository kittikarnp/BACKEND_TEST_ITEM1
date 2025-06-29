using Microsoft.EntityFrameworkCore;
using QuizIslandAPI.Models;

namespace QuizIslandAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<Choice> Choices { get; set; } = null!;
        public DbSet<Answer> Answers { get; set; } = null!;
    }
}