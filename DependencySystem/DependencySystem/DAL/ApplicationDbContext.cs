using DependencySystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.DAL   // or whatever namespace you use
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<OtpVerification> OtpVerifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .Property(e => e.ConcurrencyStamp)
                .HasColumnType("varchar(255)");

            builder.Entity<IdentityRole>()
                .Property(e => e.ConcurrencyStamp)
                .HasColumnType("varchar(255)");
            builder.Entity<Department>()
    .HasOne(d => d.Company)
    .WithMany(c => c.Departments)
    .HasForeignKey(d => d.CompanyID)
    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Department>()
                .HasIndex(d => new { d.CompanyID, d.DepartmentName })
                .IsUnique();

        }
    }
}
