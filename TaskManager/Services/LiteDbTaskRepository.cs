using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

    public async Task<TaskItem> CreateTask(string taskTitle)
    {
        var item = new TaskItem(taskTitle);
        await _context.TaskItems.AddAsync(item);
        return item;
    }

    public void Update(TaskItem task)
    {
        _context.TaskItems.Update(task);
    }

    public Task DeleteTask(TaskItem task)
    {
        _context.TaskItems.Remove(task);
        return Task.CompletedTask;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
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