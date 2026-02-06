using DependencySystem.DTOs.Company;

namespace DependencySystem.DTOs.Dashboard
{
    public class ManagerDashboardDto
    {
        public int TotalModules { get; set; }
        public int CompletedModules { get; set; }

        public int TotalTasks { get; set; }



        public double ProgressPercentage { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = "";

        public int ModuleCount { get; set; }
        public int TaskCount { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int BlockedTasks { get; set; }
        public int DeveloperCount { get; set; }

        public List<ChartItemDto> TaskStatusChart { get; set; } = new();
        public List<ChartItemDto> ModuleProgressChart { get; set; } = new();
        public List<ChartItemDto> DeveloperWorkloadChart { get; set; } = new();
    }

}
