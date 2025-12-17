using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services;

public interface ITaskRepository : IDisposable
{
    public IQueryable<TaskItem> GetAll();
    
    public Task<TaskItem> CreateTask(string taskTitle);
    
    public void Update(TaskItem task);
    
    public Task DeleteTask(TaskItem task);

    public Task Save();
}