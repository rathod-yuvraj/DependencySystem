namespace DependencySystem.DTOs.Dashboard
{
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
