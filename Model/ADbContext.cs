using Microsoft.EntityFrameworkCore;

namespace Tourism.Model
{
    public class ADbContext:DbContext
    {
        public DbSet<Admin> Adminis { get; set; }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Tour> Tours { get; set; }

        public DbSet<Travelagencies> Travelagencies { get; set; }
        public ADbContext(DbContextOptions options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Admin>().HasNoKey();
            //modelBuilder.Entity<Travelagencies>().HasNoKey();
        }

    }

}
