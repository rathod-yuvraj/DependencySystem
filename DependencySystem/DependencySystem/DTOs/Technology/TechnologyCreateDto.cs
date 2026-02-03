using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Technology
{
    public class TechnologyCreateDto
    {
        [Required]
        public string TechnologyName { get; set; } = string.Empty;
    }
}
