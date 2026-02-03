using DependencySystem.DAL;
using DependencySystem.Helpers;
using DependencySystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ DB Connection

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    ));


// ✅ Identity setup
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<DependencySystem.Services.Auth.IAuthService, DependencySystem.Services.Auth.AuthService>();
builder.Services.AddScoped<DependencySystem.Services.Auth.IEmailService, DependencySystem.Services.Auth.EmailService>();
builder.Services.AddScoped<DependencySystem.Services.Companies.ICompanyService,
                           DependencySystem.Services.Companies.CompanyService>();
builder.Services.AddScoped<DependencySystem.Services.Departments.IDepartmentService,
                           DependencySystem.Services.Departments.DepartmentService>();
builder.Services.AddScoped<DependencySystem.Services.Projects.IProjectService,
                           DependencySystem.Services.Projects.ProjectService>();
builder.Services.AddScoped<DependencySystem.Services.Modules.IModuleService,
                           DependencySystem.Services.Modules.ModuleService>();
builder.Services.AddScoped<DependencySystem.Services.Tasks.ITaskService,
                           DependencySystem.Services.Tasks.TaskService>();
builder.Services.AddScoped<
    DependencySystem.Services.Dependencies.IDependencyService,
    DependencySystem.Services.Dependencies.DependencyService>();
builder.Services.AddScoped<
    DependencySystem.Services.Teams.ITeamService,
    DependencySystem.Services.Teams.TeamService>();
builder.Services.AddScoped<
    DependencySystem.Services.Technologies.ITechnologyService,
    DependencySystem.Services.Technologies.TechnologyService>();
builder.Services.AddScoped<
    DependencySystem.Services.Audit.IAuditService,
    DependencySystem.Services.Audit.AuditService>();





var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

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


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
}
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
