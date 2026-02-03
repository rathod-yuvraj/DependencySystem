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
        public DbSet<Project> Projects { get; set; }
        public DbSet<Module> Modules { get; set; }


        public DbSet<TaskEntity> Tasks { get; set; }

        public DbSet<Dependency> Dependencies { get; set; }
        public DbSet<TaskDependency> TaskDependencies { get; set; }




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
            builder.Entity<Project>()
    .HasOne(p => p.Department)
    .WithMany()
    .HasForeignKey(p => p.DepartmentID)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Project>()
                .HasIndex(p => new { p.DepartmentID, p.ProjectName })
                .IsUnique();
            builder.Entity<Module>()
    .HasOne(m => m.Project)
    .WithMany()
    .HasForeignKey(m => m.ProjectID)
    .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<TaskEntity>().ToTable("Tasks");

            builder.Entity<TaskEntity>()
                .HasOne(t => t.Module)
                .WithMany()
                .HasForeignKey(t => t.ModuleID)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Dependency>()
    .HasOne(d => d.SourceModule)
    .WithMany()
    .HasForeignKey(d => d.SourceModuleID)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Dependency>()
                .HasOne(d => d.TargetModule)
                .WithMany()
                .HasForeignKey(d => d.TargetModuleID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Dependency>()
                .HasIndex(d => new { d.SourceModuleID, d.TargetModuleID })
                .IsUnique();
            builder.Entity<TaskDependency>()
    .HasKey(td => new { td.TaskID, td.DependsOnTaskID });

            builder.Entity<TaskDependency>()
                .HasOne(td => td.Task)
                .WithMany()
                .HasForeignKey(td => td.TaskID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskDependency>()
                .HasOne(td => td.DependsOnTask)
                .WithMany()
                .HasForeignKey(td => td.DependsOnTaskID)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
