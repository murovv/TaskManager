using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.DI;
using TaskManager.ViewModels;

namespace TaskManager.Tests.UnitTests;

[TestClass]
public class MainViewModelTests
{
    
    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var collection = new ServiceCollection();
        collection.AddDbServices(connectionString);
        collection.AddCommonServices();
        var services = collection.BuildServiceProvider();
        Ioc.Default.ConfigureServices(services);
    }
    [TestMethod]
    public void EmptyTitleTask()
    {
        var vm = new AddEditTaskViewModel() { TaskTitle = "" };
        Assert.AreEqual(false, vm.CreateTask.CanExecute(null));
    }
    
    [TestMethod]
    public void SomeTitleTask()
    {
        var vm = new AddEditTaskViewModel() { TaskTitle = "123" };
        Assert.AreEqual(true, vm.CreateTask.CanExecute(null));
    }
    
    [TestMethod]
    public void TooLongTitleTask()
    {
        var vm = new AddEditTaskViewModel() { TaskTitle = new string('x', 101) };
        Assert.AreEqual(false, vm.CreateTask.CanExecute(null));
    }
}