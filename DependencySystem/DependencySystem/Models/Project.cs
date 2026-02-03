using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DependencySystem.Models
{
    public class Project
    {
        [Key]
        public int ProjectID { get; set; }

        [Required, MaxLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [ForeignKey("Department")]
        public int DepartmentID { get; set; }

        public Department? Department { get; set; }
    }
}
