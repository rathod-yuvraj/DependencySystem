using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Dependency
{
    public class AddModuleDependencyDto
    {
        [Required]
        public int SourceModuleID { get; set; }

        [Required]
        public int TargetModuleID { get; set; }
    }
}
