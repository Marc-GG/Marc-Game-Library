using Microsoft.EntityFrameworkCore;

namespace glibraryDB.Data // Use your project namespace
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define your database tables as DbSet<T> properties
    }

    public class Game // Example model
    {

    }
}