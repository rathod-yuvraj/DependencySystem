namespace DependencySystem.DTOs.Project
{
    public class ProjectTreeResponseDto
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; } = string.Empty;

        public DepartmentMiniDto Department { get; set; } = new();
        public List<ModuleTreeDto> Modules { get; set; } = new();
    }

    public class DepartmentMiniDto
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }

    public class ModuleTreeDto
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public List<TaskTreeDto> Tasks { get; set; } = new();
    }

    public class TaskTreeDto
    {
        public int TaskID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
