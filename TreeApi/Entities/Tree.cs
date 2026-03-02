namespace TreeApi.Entities;

public class Tree
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<TreeNode> Nodes { get; set; } = new List<TreeNode>();
}