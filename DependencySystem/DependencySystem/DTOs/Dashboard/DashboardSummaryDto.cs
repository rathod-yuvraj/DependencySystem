namespace DependencySystem.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalCompanies { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalProjects { get; set; }

        public int TotalModules { get; set; }
        public int CompletedModules { get; set; }
        public int InProgressModules { get; set; }

        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }

        public double ProjectProgressPercentage { get; set; }
        public int BlockedTasks { get; set; }
    }
}
