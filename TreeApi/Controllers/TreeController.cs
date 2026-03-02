using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeApi.Data;
using TreeApi.Entities;
using TreeApi.Exceptions;
using TreeApi.Models;

namespace TreeApi.Controllers;

[ApiController]
public class TreeController : ControllerBase
{
    private readonly AppDbContext _db;

    public TreeController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("/api.user.tree.get")]
    public async Task<MNode> GetTree([FromQuery] string treeName)
    {
        var tree = await _db.Trees.FirstOrDefaultAsync(x => x.Name == treeName);

        if (tree == null)
        {
            tree = new Tree { Name = treeName };
            _db.Trees.Add(tree);
            await _db.SaveChangesAsync();
        }

        return new MNode
        {
            Id = tree.Id,
            Name = tree.Name,
            Children = BuildTree(tree.Id, null)
        };
    }

    private List<MNode> BuildTree(long treeId, long? parentId)
    {
        var nodes = _db.Nodes
            .Where(x => x.TreeId == treeId && x.ParentId == parentId)
            .ToList();

        return nodes.Select(x => new MNode
        {
            Id = x.Id,
            Name = x.Name,
            Children = BuildTree(treeId, x.Id)
        }).ToList();
    }

    [HttpPost("/api.user.tree.node.create")]
    public async Task<IActionResult> Create(
        [FromQuery] string treeName,
        [FromQuery] long? parentNodeId,
        [FromQuery] string nodeName)
    {
        var tree = await _db.Trees.FirstAsync(x => x.Name == treeName);

        if (parentNodeId.HasValue)
        {
            var parent = await _db.Nodes.FindAsync(parentNodeId.Value);
            if (parent == null || parent.TreeId != tree.Id)
                throw new SecureException("Parent not found in this tree");
        }

        _db.Nodes.Add(new TreeNode
        {
            Name = nodeName,
            ParentId = parentNodeId,
            TreeId = tree.Id
        });

        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("/api.user.tree.node.delete")]
    public async Task<IActionResult> Delete([FromQuery] long nodeId)
    {
        var node = await _db.Nodes.FindAsync(nodeId);
        if (node == null)
            throw new SecureException("Node not found");

        _db.Nodes.Remove(node);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("/api.user.tree.node.rename")]
    public async Task<IActionResult> Rename(
        [FromQuery] long nodeId,
        [FromQuery] string newNodeName)
    {
        var node = await _db.Nodes.FindAsync(nodeId);
        if (node == null)
            throw new SecureException("Node not found");

        node.Name = newNodeName;
        await _db.SaveChangesAsync();

        return Ok();
    }
}