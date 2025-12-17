using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using TaskManager.Models;
using TaskManager.Services;
using ILogger = Splat.ILogger;

namespace TaskManager.ViewModels;

public class AddEditTaskViewModel: ReactiveValidationObject
{
    /// <summary>
    /// Заголовок создаваемой задачи
    /// </summary>
    public string TaskTitle
    {
        get => _taskTitle; 
        set => this.RaiseAndSetIfChanged(ref _taskTitle, value);
    }
    
    private string _taskTitle = string.Empty;
    
    /// <summary>
    /// Список задач
    /// </summary>
    public ObservableCollection<TaskItem> TaskItems { get; }
    
    /// <summary>
    /// Команда добавления задачи
    /// </summary>
    public ICommand CreateTask { get; }

    /// <summary>
    /// Команда удаления задачи
    /// </summary>
    public ICommand DeleteTask { get; }

    /// <summary>
    /// Команда сохранения внесенных изменений
    /// </summary>
    public ICommand Save { get; }
    
    public ICommand ItemEdited { get; }
    
    public AddEditTaskViewModel()
    {
        var canExecuteCreateTask = this.WhenAnyValue(x => x.TaskTitle, (x) => !string.IsNullOrEmpty(x));
        //для того чтобы не показывать ошибку если пользователь еще ничего не вводил
        var notEmptyValidation = canExecuteCreateTask.Merge(_taskTitleNotEdited);
        this.ValidationRule(
            vm => vm.TaskTitle, 
            notEmptyValidation,
            "Заголовок не может быть пустым");
        
        var notTooLongValidation = this.WhenAnyValue(x => x.TaskTitle, (x) => x.Length <= TaskItem.TITLE_MAX_LENGTH);
        canExecuteCreateTask = canExecuteCreateTask.CombineLatest(notTooLongValidation, (a, b) => a && b);
        
        this.ValidationRule(
            vm => vm.TaskTitle, 
            notTooLongValidation,
            $"Длина заголовка не должна превышать 100 символов");
        
        CreateTask = ReactiveCommand.CreateFromTask(ExecuteCreateTask, canExecuteCreateTask);
        DeleteTask = ReactiveCommand.CreateFromTask<TaskItem>(ExecuteDeleteTask);
        ItemEdited = ReactiveCommand.CreateFromTask<TaskItem>(ExecuteEdited);
        Save = ReactiveCommand.Create(ExecuteSave);

        _taskTitleNotEdited.OnNext(true);
        _logger = Ioc.Default.GetRequiredService<ILogger<TaskItem>>();
        using (var scope = Ioc.Default.CreateScope())
        {
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var tasks = taskRepository.GetAll().Where(x => x.IsVisible).AsNoTracking().ToList();
            TaskItems = new ObservableCollection<TaskItem>(tasks);
        }
    }

    private async Task ExecuteEdited(TaskItem obj)
    {
        using (var scope = Ioc.Default.CreateScope())
        {
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            taskRepository.Update(obj);
            await taskRepository.Save();
        }
    }

    private async Task ExecuteSave()
    {
        using (var scope = Ioc.Default.CreateScope())
        {
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            await taskRepository.Save();
        }
    }

    private async Task ExecuteDeleteTask(TaskItem taskItem)
    {
        using (var scope = Ioc.Default.CreateScope())
        {
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            taskItem.IsVisible = false;
            taskRepository.Update(taskItem);
            await taskRepository.Save();
        }
        
        TaskItems.Remove(taskItem);
    }

    private async Task ExecuteCreateTask()
    {
        TaskItem newTask;
        using (var scope = Ioc.Default.CreateScope())
        {
            var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            _logger.LogInformation("Создание новой задачи");
            newTask = await taskRepository.CreateTask(TaskTitle);
            await taskRepository.Save();
        }

        TaskItems.Add(newTask);
        TaskTitle = string.Empty;
        _taskTitleNotEdited.OnNext(true);
    }
    
    private Subject<bool> _taskTitleNotEdited = new();
    
    private ILogger<TaskItem> _logger;
    
}