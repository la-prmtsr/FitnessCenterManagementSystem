using FitnessCenterManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Service> Services { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<GymSetting> GymSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Service tablosundaki Price alani icin hassasiyet ayari
            // Toplam 18 basamak , virgulden sonra 2 basamak: Orn: 1234.50
            builder.Entity<Service>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            // Bir Hizmet (Service) silindiginde, ona bagli Trainer SiLME!
            // DeleteBehavior.Restrict = Eger trainer varsa hizmeti silmeye izin verme.
            builder.Entity<Trainer>()
                .HasOne(t => t.Service)
                .WithMany()
                .HasForeignKey(t => t.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Trainer>()
                .HasOne(t => t.Service)
                .WithMany()
                .HasForeignKey(t => t.ServiceId)
                .OnDelete(DeleteBehavior.Restrict); // BU KISIM DOĞRU
        }

    }


}
