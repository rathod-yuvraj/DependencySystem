namespace DependencySystem.DTOs.Department
{
    public class DepartmentDashboardDto
    {
        public string DepartmentName { get; set; } = string.Empty;

        // ===== KPIs =====
        public int ProjectCount { get; set; }
        public int ModuleCount { get; set; }
        public int TaskCount { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int DeveloperCount { get; set; }

        // ===== Charts =====
        public List<ChartItemDto> ProjectStatusChart { get; set; } = new();
        public List<ChartItemDto> TaskStatusChart { get; set; } = new();
        public List<ChartItemDto> DeveloperWorkloadChart { get; set; } = new();
    }

    public class ChartItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
