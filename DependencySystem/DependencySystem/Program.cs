using DependencySystem.DAL;
using DependencySystem.Helper;
using DependencySystem.Models;
using DependencySystem.Services.Auth;
using DependencySystem.Services.Audit;
using DependencySystem.Services.Companies;
using DependencySystem.Services.Departments;
using DependencySystem.Services.Dependencies;
using DependencySystem.Services.Modules;
using DependencySystem.Services.Projects;
using DependencySystem.Services.Tasks;
using DependencySystem.Services.Teams;
using DependencySystem.Services.Technologies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;



// LOAD ENV (.env FIRST)
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

//// ============================
//// LOAD ENV (.env FIRST)
//// ============================
//DotNetEnv.Env.Load();

// ============================
// API VERSIONING
// ============================
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// ============================
// DATABASE
// ============================
var connectionString =
    Environment.GetEnvironmentVariable("DB_CONNECTION")
    ?? throw new Exception("DB_CONNECTION not found");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// ============================
// IDENTITY (HARDENED)
// ============================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ============================
// APPLICATION SERVICES
// ============================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IDependencyService, DependencyService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITechnologyService, TechnologyService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// ============================
// AUTHORIZATION
// ============================
builder.Services.AddScoped<IAuthorizationHandler, ProjectRoleHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProjectManagerOnly",
        p => p.Requirements.Add(new ProjectRoleRequirement("Manager")));

    options.AddPolicy("ProjectManagerOrMaintainer",
        p => p.Requirements.Add(new ProjectRoleRequirement("Manager", "Maintainer")));

    options.AddPolicy("ProjectAnyMember",
        p => p.Requirements.Add(new ProjectRoleRequirement("Manager", "Developer", "Maintainer")));
});

// ============================
// JWT AUTHENTICATION
// ============================
var jwtSettings = builder.Configuration.GetSection("Jwt");

var jwtKey =
    Environment.GetEnvironmentVariable("JWT_KEY")
    ?? throw new Exception("JWT_KEY is missing");

Console.WriteLine($"JWT KEY LENGTH: {jwtKey.Length}");

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});


// ============================
// MVC + SWAGGER
// ============================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ============================
// AUTO MIGRATIONS + SEEDING
// ============================
using (var scope = app.Services.CreateScope())
{
    try
    {
        Console.WriteLine("🟡 Startup: migrating & seeding...");

        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        await RoleSeeder.SeedRolesAsync(services);
        await AdminSeeder.SeedAdminAsync(services, builder.Configuration);

        if (!await context.Companies.AnyAsync())
        {
            Console.WriteLine("🚀 Seeding demo data...");
            await DemoDataSeeder.SeedDemoDataAsync(services, builder.Configuration);
        }

        Console.WriteLine("🟢 Startup completed");
    }
    catch (Exception ex)
    {
        Console.WriteLine("🔴 Startup failed");
        Console.WriteLine(ex);
        throw;
    }
}

// ============================
// MIDDLEWARE PIPELINE
// ============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<DependencySystem.Middlewares.ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
