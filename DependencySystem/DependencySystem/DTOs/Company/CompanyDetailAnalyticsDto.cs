namespace DependencySystem.DTOs.Company
{
    public class CompanyDetailAnalyticsDto
    {
        public string CompanyName { get; set; } = "";

        public int DepartmentCount { get; set; }
        public int ProjectCount { get; set; }
        public int ModuleCount { get; set; }
        public int TaskCount { get; set; }
        public int DeveloperCount { get; set; }

        public List<ChartItemDto> TaskStatusChart { get; set; } = new();
        public List<ChartItemDto> ProjectsPerDepartmentChart { get; set; } = new();
        public List<ChartItemDto> DeveloperWorkloadChart { get; set; } = new();
    }

    public class ChartItemDto
    {
        public string Name { get; set; } = "";
        public int Value { get; set; }
    }

}
