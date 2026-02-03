using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Technology
{
    public class AssignTechnologyDto
    {
        [Required]
        public int TechnologyID { get; set; }
    }
}
