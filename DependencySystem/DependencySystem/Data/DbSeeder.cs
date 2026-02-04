////program.cs

//using DependencySystem.DAL;
//using DependencySystem.Data;
//using DependencySystem.Models;
//using Microsoft.AspNetCore.Identity;

//var scope = app.Services.CreateScope();
//var services = scope.ServiceProvider;

//var context = services.GetRequiredService<ApplicationDbContext>();
//var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

//await DbSeeder.SeedAsync(context, userManager);



//using DependencySystem.DAL;
//using DependencySystem.Models;
//using Microsoft.AspNetCore.Identity;

//namespace DependencySystem.Data
//{
   

//    public static class DbSeeder
//    {
//        public static async Task SeedAsync(
//            ApplicationDbContext context,
//            UserManager<ApplicationUser> userManager)
//        {
//            if (context.Projects.Any())
//                return;

//            // ========================
//            // TEAMS
//            // ========================
//            var teams = new List<Team>();
//            for (int i = 1; i <= 5; i++)
//            {
//                teams.Add(new Team
//                {
//                    Name = $"Team {i}",
//                    Description = $"Development Team {i}"
//                });
//            }
//            context.Teams.AddRange(teams);
//            await context.SaveChangesAsync();

//            // ========================
//            // USERS
//            // ========================
//            for (int i = 1; i <= 25; i++)
//            {
//                var user = new ApplicationUser
//                {
//                    UserName = $"user{i}",
//                    Email = $"user{i}@demo.com",
//                    IsVerified = true,
//                    TeamId = teams[i % 5].Id
//                };
//                await userManager.CreateAsync(user, "Password@123");
//            }

//            // ========================
//            // PROJECTS
//            // ========================
//            var projects = new List<Project>();
//            for (int i = 1; i <= 5; i++)
//            {
//                projects.Add(new Project
//                {
//                    Name = $"Project {i}",
//                    Description = $"Enterprise Project {i}",
//                    StartDate = DateTime.UtcNow.AddDays(-30),
//                    EndDate = DateTime.UtcNow.AddMonths(6)
//                });
//            }
//            context.Projects.AddRange(projects);
//            await context.SaveChangesAsync();

//            // ========================
//            // MODULES
//            // ========================
//            var modules = new List<Module>();
//            int moduleCounter = 1;

//            foreach (var project in projects)
//            {
//                for (int i = 1; i <= 6; i++)
//                {
//                    modules.Add(new Module
//                    {
//                        Name = $"Module {moduleCounter}",
//                        ProjectId = project.Id
//                    });
//                    moduleCounter++;
//                }
//            }
//            context.Modules.AddRange(modules);
//            await context.SaveChangesAsync();

//            // ========================
//            // TASKS
//            // ========================
//            var tasks = new List<ProjectTask>();
//            int taskCounter = 1;

//            foreach (var module in modules)
//            {
//                for (int i = 1; i <= 2; i++)
//                {
//                    tasks.Add(new ProjectTask
//                    {
//                        Title = $"Task {taskCounter}",
//                        Description = $"Implementation Task {taskCounter}",
//                        ModuleId = module.Id,
//                        Status = TaskStatus.Pending
//                    });
//                    taskCounter++;
//                }
//            }
//            context.ProjectTasks.AddRange(tasks);
//            await context.SaveChangesAsync();

//            // ========================
//            // MODULE DEPENDENCIES (DAG)
//            // ========================
//            var moduleDeps = new List<ModuleDependency>();

//            for (int i = 1; i < modules.Count; i++)
//            {
//                moduleDeps.Add(new ModuleDependency
//                {
//                    ModuleId = modules[i].Id,
//                    DependsOnModuleId = modules[i - 1].Id
//                });
//            }
//            context.ModuleDependencies.AddRange(moduleDeps);

//            // ========================
//            // TASK DEPENDENCIES (DAG)
//            // ========================
//            var taskDeps = new List<TaskDependency>();

//            for (int i = 1; i < tasks.Count; i++)
//            {
//                taskDeps.Add(new TaskDependency
//                {
//                    TaskId = tasks[i].Id,
//                    DependsOnTaskId = tasks[i - 1].Id
//                });
//            }
//            context.TaskDependencies.AddRange(taskDeps);

//            await context.SaveChangesAsync();
//        }
//    }

//}




