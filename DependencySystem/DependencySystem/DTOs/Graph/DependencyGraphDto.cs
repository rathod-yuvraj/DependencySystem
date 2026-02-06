namespace DependencySystem.DTOs.Graph
{
    public class DependencyGraphDto
    {
        public List<GraphNodeDto> Nodes { get; set; } = new();
        public List<GraphEdgeDto> Edges { get; set; } = new();
    }

    public class GraphNodeDto
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;

        // "Module" | "Task"
        public string Type { get; set; } = string.Empty;

        // Optional: Active / Pending / Blocked
        public string Status { get; set; } = string.Empty;
    }

    public class GraphEdgeDto
    {
        public int Source { get; set; }
        public int Target { get; set; }

        // "MODULE_DEPENDENCY" | "TASK_DEPENDENCY"
        public string Relation { get; set; } = string.Empty;
    }
}
