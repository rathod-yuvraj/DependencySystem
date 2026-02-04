namespace DependencySystem.DTOs.Company
{
    public class CompanyResponseDto
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public List<DepartmentSimpleDto> Departments { get; set; } = new();
    }

    public class DepartmentSimpleDto
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int CompanyID { get; set; }
    }
}
