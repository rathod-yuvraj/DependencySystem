////program
////using DependencySystem.DAL;
////using DependencySystem.Data;
////using DependencySystem.Models;
////using Microsoft.AspNetCore.Identity;

////using (var scope = app.Services.CreateScope())
////{
////    var services = scope.ServiceProvider;
////    var context = services.GetRequiredService<ApplicationDbContext>();
////    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

////    await DbSeeder.SeedAsync(context, userManager);
////}

////program
//using DependencySystem.DAL;
//using DependencySystem.Models;
//using Microsoft.AspNetCore.Identity;

//namespace DependencySystem.Data
//{
//    public static class DbSeeders
//    {
//        public static async Task SeedAsync(
//            ApplicationDbContext context,
//            UserManager<ApplicationUser> userManager)
//        {
//            if (context.Projects.Any()) return; // prevent reseed

//            // =========================
//            // USERS (10)
//            // =========================
//            var users = new List<ApplicationUser>();

//            for (int i = 1; i <= 10; i++)
//            {
//                var user = new ApplicationUser
//                {
//                    UserName = $"user{i}",
//                    Email = $"user{i}@demo.com",
//                    IsVerified = true
//                };

//                await userManager.CreateAsync(user, "Password@123");
//                users.Add(user);
//            }

//            // =========================
//            // TEAMS (5)
//            // =========================
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

//            // =========================
//            // PROJECTS (5)
//            // =========================
//            var projects = new List<Project>();
//            for (int i = 1; i <= 5; i++)
//            {
//                projects.Add(new Project
//                {
//                    Name = $"Project {i}",
//                    Description = $"Enterprise Dependency Project {i}",
//                    StartDate = DateTime.UtcNow.AddDays(-30),
//                    EndDate = DateTime.UtcNow.AddMonths(3)
//                });
//            }

//            context.Projects.AddRange(projects);
//            await context.SaveChangesAsync();

//            // =========================
//            // MODULES (25)
//            // =========================
//            var modules = new List<Module>();
//            int moduleCounter = 1;

//            foreach (var project in projects)
//            {
//                for (int i = 1; i <= 5; i++)
//                {
//                    modules.Add(new Module
//                    {
//                        Name = $"Module {moduleCounter}",
//                        ProjectId = project.Id,
//                        TeamId = teams[(moduleCounter - 1) % teams.Count].Id
//                    });
//                    moduleCounter++;
//                }
//            }

//            context.Modules.AddRange(modules);
//            await context.SaveChangesAsync();

//            // =========================
//            // MODULE DEPENDENCIES (DAG)
//            // =========================
//            var moduleDeps = new List<ModuleDependency>();

//            foreach (var project in projects)
//            {
//                var projectModules = modules
//                    .Where(m => m.ProjectId == project.Id)
//                    .ToList();

//                for (int i = 1; i < projectModules.Count; i++)
//                {
//                    moduleDeps.Add(new ModuleDependency
//                    {
//                        ModuleId = projectModules[i].Id,
//                        DependsOnModuleId = projectModules[i - 1].Id
//                    });
//                }
//            }

//            context.ModuleDependencies.AddRange(moduleDeps);
//            await context.SaveChangesAsync();

//            // =========================
//            // TASKS (75+)
//            // =========================
//            var tasks = new List<TaskItem>();
//            int taskCounter = 1;

//            foreach (var module in modules)
//            {
//                for (int i = 1; i <= 3; i++)
//                {
//                    tasks.Add(new TaskItem
//                    {
//                        Title = $"Task {taskCounter}",
//                        Description = $"Task {taskCounter} for {module.Name}",
//                        ModuleId = module.Id,
//                        AssignedUserId = users[taskCounter % users.Count].Id,
//                        Status = "Pending"
//                    });
//                    taskCounter++;
//                }
//            }

//            context.Tasks.AddRange(tasks);
//            await context.SaveChangesAsync();

//            // =========================
//            // TASK DEPENDENCIES (DAG)
//            // =========================
//            var taskDeps = new List<TaskDependency>();

//            foreach (var module in modules)
//            {
//                var moduleTasks = tasks
//                    .Where(t => t.ModuleId == module.Id)
//                    .ToList();

//                for (int i = 1; i < moduleTasks.Count; i++)
//                {
//                    taskDeps.Add(new TaskDependency
//                    {
//                        TaskId = moduleTasks[i].Id,
//                        DependsOnTaskId = moduleTasks[i - 1].Id
//                    });
//                }
//            }

//            context.TaskDependencies.AddRange(taskDeps);
//            await context.SaveChangesAsync();
//        }
//    }
//}
