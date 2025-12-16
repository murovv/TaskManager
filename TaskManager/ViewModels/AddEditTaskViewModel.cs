using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
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
    
    public ObservableCollection<TaskItem> TaskItems { get; }
    public ICommand CreateTask { get; }

    public ICommand DeleteTask { get; }

    public ICommand Save { get; }
    
    public AddEditTaskViewModel()
    {
        var canExecuteCreateTask = this.WhenAnyValue(x => x.TaskTitle, (x) => !string.IsNullOrEmpty(x));
        //для того чтобы не показывать ошибку если пользователь еще ничего не вводил
        var validationRule = canExecuteCreateTask.Merge(_taskTitleNotEdited);
        
        this.ValidationRule(vm => vm.TaskTitle, validationRule, "Заголовок не может быть пустым");
        CreateTask = ReactiveCommand.Create(ExecuteCreateTask, canExecuteCreateTask);
        DeleteTask = ReactiveCommand.Create<TaskItem>(ExecuteDeleteTask);
        Save = ReactiveCommand.Create(ExecuteSave);

        _logger = Ioc.Default.GetRequiredService<ILogger<TaskItem>>();
        _taskRepository = Ioc.Default.GetRequiredService<ITaskRepository>();
        _taskTitleNotEdited.OnNext(true);

        var tasks = _taskRepository.GetAll().ToList();
        TaskItems = new ObservableCollection<TaskItem>(tasks);
    }

    private void ExecuteSave()
    {
        _taskRepository.Save();
    }

    private void ExecuteDeleteTask(TaskItem taskItem)
    {
        //soft delete - не удаляем из бд
        TaskItems.Remove(taskItem);
    }

    private void ExecuteCreateTask()
    {
        _logger.LogInformation("Creating new task");
        var newTask = _taskRepository.CreateTask(TaskTitle);
        _taskRepository.Save();

        TaskItems.Add(newTask);
        TaskTitle = string.Empty;
        _taskTitleNotEdited.OnNext(true);
    }

    private ITaskRepository _taskRepository;
    
    private IObservable<bool> _taskTitleIsEmpty;
    
    private Subject<bool> _taskTitleNotEdited = new();
    
    private ILogger<TaskItem> _logger;
    
}