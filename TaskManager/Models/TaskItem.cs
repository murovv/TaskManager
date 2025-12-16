using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class TaskItem
{
    public const int TITLE_MAX_LENGTH = 100;
    
    public Guid Id { get; private set; }
    
    [Required]
    [StringLength(TITLE_MAX_LENGTH, MinimumLength = 1)]
    public string Title { get; set; }
    public bool IsCompleted { get; set; }

    public bool IsVisible { get; set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }

    public TaskItem(string title):this()
    {
        Title = title;
        CreatedAt = DateTime.Now;
    }

    private TaskItem()
    {
    }
}