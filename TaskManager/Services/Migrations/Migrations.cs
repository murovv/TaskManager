using System;
using System.Linq;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TaskManager.Services.Migrations;

public class Migrations
{
    public static void ApplyTaskMigrations()
    {
        using (var scope = Ioc.Default.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskContext>();
            var log = scope.ServiceProvider.GetRequiredService<ILogger<TaskContext>>();
            // Check and apply pending migrations
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
}