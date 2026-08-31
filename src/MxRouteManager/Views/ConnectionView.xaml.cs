using System.Windows.Controls;
using MxRouteManager.ViewModels;

namespace MxRouteManager.Views;

public partial class ConnectionView : UserControl
{
    private bool _syncing;

    public ConnectionView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        ApiKeyBox.PasswordChanged += OnPasswordChanged;
    }

    private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is ConnectionViewModel vm)
        {
            _syncing = true;
            if (ApiKeyBox.Password != vm.ApiKey)
                ApiKeyBox.Password = vm.ApiKey;
            _syncing = false;

            vm.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(ConnectionViewModel.ApiKey) && !_syncing)
                {
                    if (ApiKeyBox.Password != vm.ApiKey)
                    {
                        _syncing = true;
                        ApiKeyBox.Password = vm.ApiKey;
                        _syncing = false;
                    }
                }
            };
        }
    }

    private void OnPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_syncing) return;
        if (DataContext is ConnectionViewModel vm)
            vm.ApiKey = ApiKeyBox.Password;
    }
}
