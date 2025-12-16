using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    public void EmptyTitleTask()
    {
        /*using (var repo = new LiteDbTaskRepository(new TaskContext()))
        {
            
            repo.CreateTask("");
            repo.Save(); 
        }*/
    }
}