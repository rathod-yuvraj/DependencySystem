namespace DependencySystem.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public OrganizationStats Organization { get; set; } = new();
        public ModuleStats Modules { get; set; } = new();
        public TaskStats Tasks { get; set; } = new();

        public double ProjectProgressPercentage { get; set; }
        public int BlockedTasks { get; set; }
    }

    public class OrganizationStats
    {
        public int TotalCompanies { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalProjects { get; set; }
    }

    public class ModuleStats
    {
        public int TotalModules { get; set; }
        public int CompletedModules { get; set; }
        public int InProgressModules { get; set; }
    }

    public class TaskStats
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
    }
}
