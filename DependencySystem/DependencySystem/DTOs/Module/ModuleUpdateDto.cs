namespace DependencySystem.DTOs.Module
{
    public class ModuleUpdateDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public int ProjectID { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
