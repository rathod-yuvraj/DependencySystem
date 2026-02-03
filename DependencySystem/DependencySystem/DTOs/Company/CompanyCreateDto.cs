using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Company
{
    public class CompanyCreateDto
    {
        [Required]
        public string CompanyName { get; set; } = string.Empty;
    }
}
