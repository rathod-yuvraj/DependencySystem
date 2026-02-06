namespace DependencySystem.DTOs.Company
{
    public class CompanyAnalyticsDto
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; } = "";

        public int DepartmentCount { get; set; }
        public int ProjectCount { get; set; }
        public int ModuleCount { get; set; }
        public int TaskCount { get; set; }

        public List<string> Developers { get; set; } = new();
    }
}
