namespace DependencySystem.DTOs.Dependency
{
    public class DependencyGraphDto
    {
        public List<NodeDto> Nodes { get; set; } = new();
        public List<EdgeDto> Edges { get; set; } = new();
    }

    public class NodeDto
    {
        public string Id { get; set; } = string.Empty; // e.g. "M1", "T10"
        public string Label { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Module / Task
        public string Status { get; set; } = string.Empty;
    }

    public class EdgeDto
    {
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public string Relation { get; set; } = "depends_on";
    }
}
