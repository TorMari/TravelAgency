using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebProjectServ.Models
{
    public class MyDataContext : IdentityDbContext<ApplicationUser> 
    {

        public MyDataContext(DbContextOptions<MyDataContext> options) 
            : base(options) 
        { 
            Database.EnsureCreated();
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(l => l.Client)
                .WithMany(r => r.Bookings)
                .HasForeignKey(l => l.ClientId);

            modelBuilder.Entity<Booking>()
                .HasOne(l => l.Tour)
                .WithMany(b => b.Bookings)
                .HasForeignKey(l => l.TourId);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.ClientId, b.TourId })
                .IsUnique();

            modelBuilder.Entity<Message>()
            .HasIndex(m => m.TourId);           

            modelBuilder.Entity<Message>()
                .HasIndex(m => new { m.SenderId, m.ReceiverId }); 
        }
    }
}
