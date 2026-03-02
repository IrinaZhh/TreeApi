using Microsoft.EntityFrameworkCore;
using TreeApi.Entities;

namespace TreeApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    public DbSet<Tree> Trees { get; set; }
    public DbSet<TreeNode> Nodes { get; set; }
    public DbSet<ExceptionJournal> Journals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tree: уникальное имя
        modelBuilder.Entity<Tree>()
            .HasIndex(t => t.Name)
            .IsUnique();

        // TreeNode: уникальная комбинация родителя и имени
        modelBuilder.Entity<TreeNode>()
            .HasIndex(n => new { n.ParentId, n.Name })
            .IsUnique();

        // Self-reference (ParentId -> Id) без каскадного удаления
        modelBuilder.Entity<TreeNode>()
            .HasOne(n => n.Parent)
            .WithMany(n => n.Children)
            .HasForeignKey(n => n.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // или DeleteBehavior.NoAction

        // Tree -> Nodes: оставляем каскадное удаление
        modelBuilder.Entity<TreeNode>()
            .HasOne(n => n.Tree)
            .WithMany(t => t.Nodes)
            .HasForeignKey(n => n.TreeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}