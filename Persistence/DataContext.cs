using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        // public DbSet<Activity> Activites { get; set; }
        public DbSet<Role> Role { get; set; }
    }
}