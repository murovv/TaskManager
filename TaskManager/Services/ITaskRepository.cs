using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services;

public interface ITaskRepository : IDisposable
{
    public IQueryable<TaskItem> GetAll();
    
    public TaskItem CreateTask(string taskTitle);
    
    public void DeleteTask(TaskItem task);

    public void Save();
}