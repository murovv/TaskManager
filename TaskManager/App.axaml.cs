using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskManager.DI;
using TaskManager.Services;
using TaskManager.ViewModels;
using TaskManager.Views;

namespace TaskManager;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
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
        
        /*if(InitDatabase())*/
        ApplyMigrations();
        
        var vm = Ioc.Default.GetRequiredService<MainViewViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm,
            };
        }

        Dispatcher.UIThread.UnhandledException += (s, e) =>
        {
            var appLog = Ioc.Default.GetRequiredService<ILogger<App>>();
            appLog.LogCritical(e.Exception, "Необработанное исключение");
            e.Handled = true;
        };
        
        base.OnFrameworkInitializationCompleted();
    }
    
    private void ApplyMigrations()
    {
        using var scope = Ioc.Default.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskContext>();
        var log = scope.ServiceProvider.GetRequiredService<ILogger<TaskContext>>();
        
        var pendingMigrations = dbContext.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            log.LogInformation($"Применение миграций");
            dbContext.Database.Migrate();
        }
        else
        {
            log.LogInformation("Схема бд актуальна.");
        }
    }
}