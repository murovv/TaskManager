
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Services;

public class TaskContext : DbContext
{
    public TaskContext(DbContextOptions<TaskContext> options) : base(options)
    {
    }

    public TaskContext()
    {
        Database.EnsureCreated();
    }
    public DbSet<TaskItem> TaskItems { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=tasks.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>().Property(t => t.Title).IsRequired().HasMaxLength(10);
    }
}
