using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DependencySystem.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }

        [Required, MaxLength(200)]
        public string DepartmentName { get; set; } = string.Empty;

        [ForeignKey("Company")]
        public int CompanyID { get; set; }

        public Company? Company { get; set; }
    }
}
