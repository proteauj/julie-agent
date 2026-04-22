using Microsoft.EntityFrameworkCore;
using Memora.Api.Models;

namespace Memora.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<FacilityAdmin> FacilityAdmins { get; set; } 
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ici tu mets ton code de configuration des relations personnalisées.
            // Par exemple (attention, pas forcément besoin pour User/Facility si conventions respectées) :
            modelBuilder.Entity<Facility>()
                .HasMany(f => f.Seniors)
                .WithOne(u => u.Facility)
                .HasForeignKey(u => u.FacilityId);

            modelBuilder.Entity<FacilityAdmin>()
                .HasOne(fa => fa.User)
                .WithMany() // ou WithMany(u => u.FacilityAdmins) si tu exposes côté User
                .HasForeignKey(fa => fa.UserId);
            modelBuilder.Entity<FacilityAdmin>()
                .HasOne(fa => fa.Facility)
                .WithMany(f => f.FacilityAdmins)
                .HasForeignKey(fa => fa.FacilityId);

            modelBuilder.Entity<Reminder>()
                .Property(r => r.Title)
                .HasMaxLength(150);

            modelBuilder.Entity<Reminder>()
                .Property(r => r.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Reminder>()
                .Property(r => r.Type)
                .HasMaxLength(30);

            modelBuilder.Entity<Reminder>()
                .Property(r => r.Channel)
                .HasMaxLength(20);

            modelBuilder.Entity<Reminder>()
                .Property(r => r.RecurrenceRule)
                .HasMaxLength(50);

            modelBuilder.Entity<Reminder>()
                .Property(r => r.SourceType)
                .HasMaxLength(30);

            modelBuilder.Entity<Reminder>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}