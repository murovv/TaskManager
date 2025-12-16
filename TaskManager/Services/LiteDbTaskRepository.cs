using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services;

public class LiteDbTaskRepository : ITaskRepository
{
    public LiteDbTaskRepository(TaskContext context)
    {
        _context = context;
    }
    
    public IQueryable<TaskItem> GetAll()
    {
        return _context.TaskItems.AsQueryable();
    }

    public TaskItem CreateTask(string taskTitle)
    {
        var item = new TaskItem(taskTitle);
        _context.TaskItems.Add(item);
        return item;
    }

    public void DeleteTask(TaskItem task)
    {
        _context.TaskItems.Remove(task);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
    
    private TaskContext _context;
    
    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        this._disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}