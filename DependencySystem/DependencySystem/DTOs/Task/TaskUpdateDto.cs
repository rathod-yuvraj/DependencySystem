namespace DependencySystem.DTOs.Task
{
    public class TaskUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;   // Enum as string
        public int ModuleID { get; set; }
    }
}
