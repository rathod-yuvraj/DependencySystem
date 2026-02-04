namespace DependencySystem.DTOs.Dashboard
{
    // ===================== ADMIN =====================
    public class AdminDashboardDto
    {
        public int TotalCompanies { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalProjects { get; set; }
        public int TotalUsers { get; set; }
        public int TotalBlockedTasks { get; set; }
    }

    // ===================== MANAGER =====================
    public class ManagerDashboardDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;

        public int TotalModules { get; set; }
        public int CompletedModules { get; set; }

        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int BlockedTasks { get; set; }

        public double ProgressPercentage { get; set; }
    }

    // ===================== DEVELOPER =====================
    public class DeveloperDashboardDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        public int TotalAssignedTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int BlockedTasks { get; set; }
    }
}
