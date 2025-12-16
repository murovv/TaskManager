using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManager.Services;
using TaskManager.ViewModels;

namespace TaskManager.DI;

public static class Bootstrapper
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddScoped<ITaskRepository, LiteDbTaskRepository>();
        collection.AddTransient<MainViewViewModel>();
        collection.AddLogging(builder =>builder.AddConsole());
    }

    public static void AddDbServices(this IServiceCollection collection, string connectionString)
    {
        collection.AddDbContext<TaskContext>(options =>
        {
            options.UseSqlite("Data Source=tasks.db");
            //options.UseNpgsql(connectionString);
        });
    }
}