namespace DependencySystem.Models
{
    public class ModuleTechnology
    {
        public int ModuleID { get; set; }
        public Module? Module { get; set; }

        public int TechnologyID { get; set; }
        public Technology? Technology { get; set; }
    }
}
