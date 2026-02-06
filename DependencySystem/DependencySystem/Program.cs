using DependencySystem.DAL;
using DependencySystem.Helper;
using DependencySystem.Hubs;
using DependencySystem.Models;
using DependencySystem.Services.Audit;
using DependencySystem.Services.Auth;
using DependencySystem.Services.Companies;
using DependencySystem.Services.Dashboard;
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
using System.Text.Json;
using System.Text.Json.Serialization;



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
//builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IRoleDashboardService, RoleDashboardService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddSignalR();


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
var jwtKey =
    Environment.GetEnvironmentVariable("JWT_KEY")
    ?? throw new Exception("JWT_KEY missing");

var jwtIssuer =
    Environment.GetEnvironmentVariable("JWT_ISSUER")
    ?? throw new Exception("JWT_ISSUER missing");

var jwtAudience =
    Environment.GetEnvironmentVariable("JWT_AUDIENCE")
    ?? throw new Exception("JWT_AUDIENCE missing");

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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

});



// ============================
// MVC + SWAGGER
// ============================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Prevent circular reference explosions
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        // Production: compact JSON (smaller payloads)
        options.JsonSerializerOptions.WriteIndented = false;

        // Optional but recommended
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173", // Vite
                "http://localhost:3000"  // CRA
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();

// ============================
// AUTO MIGRATIONS + SEEDING
// ============================
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
//    });

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var config = services.GetRequiredService<IConfiguration>();

    var db = services.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    // Run relational demo seeding
    await DependencySystem.Seeding.RelationalDemoSeeder
        .SeedAsync(services, config);
}

// ============================
// MIDDLEWARE PIPELINE
// ============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapHub<ProjectHub>("/hubs/project");

app.UseHttpsRedirection();

app.UseCors("AllowFrontend"); // ✅ ADD THIS LINE

app.UseMiddleware<DependencySystem.Middlewares.ExceptionMiddleware>();

app.UseAuthentication();   // FIRST
app.UseAuthorization();    // SECOND

app.MapControllers();
app.Run();