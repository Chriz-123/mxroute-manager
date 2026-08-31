using System.Windows;
using System.Windows.Threading;

namespace MxRouteManager;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += OnUnhandledException;
    }

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            "Ein unerwarteter Fehler ist aufgetreten:\n\n" + e.Exception.Message,
            "MXroute Manager", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
