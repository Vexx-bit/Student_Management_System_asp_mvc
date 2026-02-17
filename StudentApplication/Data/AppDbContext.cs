using Microsoft.EntityFrameworkCore;
using StudentApplication.Models;

namespace StudentApplication.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor needed for Dependency Injection
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // This property creates the 'Students' table in the database
        public DbSet<Student> Students { get; set; }
    }
}