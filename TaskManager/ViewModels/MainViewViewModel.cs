using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace TaskManager.ViewModels;

public class MainViewViewModel : ReactiveObject
{
    public AddEditTaskViewModel AddEditTaskViewModel { get; } = new();
}