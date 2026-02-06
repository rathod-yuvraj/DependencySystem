namespace DependencySystem.DTOs.Task
{
    public class TaskResponseDto
    {
        public int TaskID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ModuleID { get; set; }
    }
}
