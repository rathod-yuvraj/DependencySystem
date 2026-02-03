using System.ComponentModel.DataAnnotations.Schema;

namespace DependencySystem.Models
{
    public class TaskDependency
    {
        public int TaskID { get; set; }
        public TaskEntity? Task { get; set; }

        public int DependsOnTaskID { get; set; }
        public TaskEntity? DependsOnTask { get; set; }
    }
}
