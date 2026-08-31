using System.Windows;
using System.Windows.Threading;
using MxRouteManager.Localization;

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
            Loc.T("App_UnexpectedError", e.Exception.Message),
            "MXroute Manager", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
