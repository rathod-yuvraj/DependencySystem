using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DependencySystem.Models
{
    public class Dependency
    {
        [Key]
        public int DependencyID { get; set; }

        [ForeignKey("SourceModule")]
        public int SourceModuleID { get; set; }
        public Module? SourceModule { get; set; }

        [ForeignKey("TargetModule")]
        public int TargetModuleID { get; set; }
        public Module? TargetModule { get; set; }
    }
}
