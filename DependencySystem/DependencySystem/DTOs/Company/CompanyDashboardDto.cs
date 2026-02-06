namespace DependencySystem.DTOs.Company
{
    public class CompanyDashboardDto
    {
        public string CompanyName { get; set; } = "";

        public int DepartmentCount { get; set; }
        public int ProjectCount { get; set; }
        public int ModuleCount { get; set; }
        public int TaskCount { get; set; }
        public int DeveloperCount { get; set; }

        public List<ChartItemDto> ProjectsPerDepartment { get; set; } = new();
        public List<ChartItemDto> TaskStatus { get; set; } = new();
    }

}
