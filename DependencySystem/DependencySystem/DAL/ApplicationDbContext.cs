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
        public DbSet<TeamMemberProfile> TeamMemberProfiles { get; set; }
        public DbSet<ProjectTeamMember> ProjectTeamMembers { get; set; }
        public DbSet<Technology> Technologies { get; set; }
        public DbSet<ProjectTechnology> ProjectTechnologies { get; set; }
        public DbSet<ModuleTechnology> ModuleTechnologies { get; set; }
        public DbSet<UserTechnology> UserTechnologies { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }



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

            // ===================== USER → PROFILE (1-1) =====================
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.TeamMemberProfile)
                .WithOne(p => p.User)
                .HasForeignKey<TeamMemberProfile>(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // ===================== PROFILE → DEPARTMENT (Many Profiles → 1 Department) =====================
            builder.Entity<TeamMemberProfile>()
                .HasOne(p => p.Department)
                .WithMany()
                .HasForeignKey(p => p.DepartmentID)
                .OnDelete(DeleteBehavior.SetNull);

            // ===================== PROJECT ↔ USER (Many-to-Many with Role) =====================
            builder.Entity<ProjectTeamMember>()
                .HasKey(x => new { x.ProjectID, x.UserID });

            builder.Entity<ProjectTeamMember>()
                .HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProjectTeamMember>()
                .HasOne(x => x.User)
                .WithMany(u => u.ProjectTeamMembers)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ProjectTechnology>()
    .HasKey(x => new { x.ProjectID, x.TechnologyID });

            builder.Entity<ModuleTechnology>()
                .HasKey(x => new { x.ModuleID, x.TechnologyID });

            builder.Entity<UserTechnology>()
                .HasKey(x => new { x.UserID, x.TechnologyID });

        }
    }
}
