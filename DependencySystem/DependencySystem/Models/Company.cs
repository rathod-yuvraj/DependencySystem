using System.ComponentModel.DataAnnotations;

namespace DependencySystem.Models
{
    public class Company
    {
        [Key]
        public int CompanyID { get; set; }

        [Required, MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
