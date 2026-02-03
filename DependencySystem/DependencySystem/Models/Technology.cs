using System.ComponentModel.DataAnnotations;

namespace DependencySystem.Models
{
    public class Technology
    {
        [Key]
        public int TechnologyID { get; set; }

        [Required, MaxLength(150)]
        public string TechnologyName { get; set; } = string.Empty;
    }
}
