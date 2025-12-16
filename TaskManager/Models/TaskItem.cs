using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class TaskItem
{
    public Guid Id { get; private set; }
    
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; }      // обязательно, до 100 символов
    public bool IsCompleted { get; set; }
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