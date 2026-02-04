namespace DependencySystem.DTOs.Department
{
    public class DepartmentResponseDto
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public CompanyMiniDto Company { get; set; } = new();
        public List<ProjectMiniDto> Projects { get; set; } = new();
    }

    public class CompanyMiniDto
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }

    public class ProjectMiniDto
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; } = string.Empty;
    }
}
