using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Tests.UnitTests;

[TestClass]
public class RepositoryTests
{
    [TestInitialize]
    public void Setup()
    {
    }
    
    [TestMethod]
    [ExpectedException(typeof(DbUpdateException), AllowDerivedTypes = true)]
    public async Task EmptyTitleTask()
    {
        using (var repo = new LiteDbTaskRepository(new TaskContext()))
        {
            await repo.CreateTask("");
            await repo.Save(); 
        }
    }
    
    [TestMethod]
    [ExpectedException(typeof(DbUpdateException), AllowDerivedTypes = true)]
    public async Task TooLongTitelTask()
    {
        using (var repo = new LiteDbTaskRepository(new TaskContext()))
        {
            await repo.CreateTask(new string('*', 101));
            await repo.Save(); 
        }
    }
    
    [TestMethod]
    public async Task TaskItemAddsIn()
    {
        TaskItem newTask;
        using (var repo = new LiteDbTaskRepository(new TaskContext()))
        {
            newTask = await repo.CreateTask(new string('*', 50));
            await repo.Save(); 
        }
        
        using (var repo = new LiteDbTaskRepository(new TaskContext()))
        {
            var foundItem = repo.GetAll().FirstOrDefault(x=>x.Id == newTask.Id);
            Assert.IsNotNull(foundItem);
        }
    }
}