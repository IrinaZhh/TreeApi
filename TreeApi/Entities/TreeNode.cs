namespace TreeApi.Entities;

public class TreeNode
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;

    public long TreeId { get; set; }
    public Tree Tree { get; set; } = null!;

    public long? ParentId { get; set; }
    public TreeNode? Parent { get; set; }

    public ICollection<TreeNode> Children { get; set; } = new List<TreeNode>();
}