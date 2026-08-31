using System.Windows;
using MxRouteManager.ViewModels;

namespace MxRouteManager;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();
        _vm = new MainViewModel();
        DataContext = _vm;
        Loaded += async (_, _) => await _vm.InitializeAsync();
    }
}
