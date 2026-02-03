using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Module
{
    public class ModuleCreateDto
    {
        [Required]
        public string ModuleName { get; set; } = string.Empty;

        [Required]
        public int ProjectID { get; set; }
    }
}
