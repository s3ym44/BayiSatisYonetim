using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BayiSatisYonetim.Models.Entities;

namespace BayiSatisYonetim.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.FullName).HasMaxLength(200);
            });

            builder.Entity<Dealer>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithOne(u => u.Dealer)
                    .HasForeignKey<Dealer>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(d => d.CommissionRate).HasPrecision(5, 2);
                entity.Property(d => d.CompanyName).HasMaxLength(300);
                entity.Property(d => d.TaxNumber).HasMaxLength(20);
                entity.Property(d => d.City).HasMaxLength(100);
                entity.Property(d => d.IBAN).HasMaxLength(34);
            });

            builder.Entity<Customer>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithOne(u => u.Customer)
                    .HasForeignKey<Customer>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.TCKimlik).HasMaxLength(11);
                entity.Property(c => c.City).HasMaxLength(100);
            });

            builder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(200);
            });

            builder.Entity<Company>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(200);
            });

            builder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Company)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.Price).HasPrecision(10, 2);
                entity.Property(p => p.Name).HasMaxLength(300);
            });

            builder.Entity<Application>(entity =>
            {
                entity.HasOne(a => a.Customer)
                    .WithMany(c => c.Applications)
                    .HasForeignKey(a => a.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Dealer)
                    .WithMany(d => d.Applications)
                    .HasForeignKey(a => a.DealerId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(a => a.Product)
                    .WithMany(p => p.Applications)
                    .HasForeignKey(a => a.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(a => a.ApplicationNumber).HasMaxLength(20);
                entity.HasIndex(a => a.ApplicationNumber).IsUnique();
            });

            builder.Entity<Sale>(entity =>
            {
                entity.HasOne(s => s.Application)
                    .WithOne(a => a.Sale)
                    .HasForeignKey<Sale>(s => s.ApplicationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Dealer)
                    .WithMany(d => d.Sales)
                    .HasForeignKey(s => s.DealerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Product)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(s => s.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(s => s.Amount).HasPrecision(10, 2);
                entity.Property(s => s.CommissionAmount).HasPrecision(10, 2);
            });

            builder.Entity<ActivityLog>(entity =>
            {
                entity.HasOne(a => a.User)
                    .WithMany(u => u.ActivityLogs)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
