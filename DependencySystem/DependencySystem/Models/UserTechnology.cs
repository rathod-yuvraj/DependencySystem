namespace DependencySystem.Models
{
    public class UserTechnology
    {
        public string UserID { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int TechnologyID { get; set; }
        public Technology? Technology { get; set; }
    }
}
