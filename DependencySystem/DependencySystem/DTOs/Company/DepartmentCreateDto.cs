using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Department
{
    public class DepartmentCreateDto
    {
        [Required]
        public string DepartmentName { get; set; } = string.Empty;

        [Required]
        public int CompanyID { get; set; }
    }
}
