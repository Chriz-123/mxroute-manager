using CommunityToolkit.Mvvm.ComponentModel;
using MxRouteManager.Services;

namespace MxRouteManager.ViewModels;

/// <summary>Basis fuer alle Inhalts-Seiten. Kapselt Ladezustand und Fehleranzeige.</summary>
public abstract partial class PageViewModelBase : ObservableObject
{
    protected readonly MainViewModel Shell;
    protected MxRouteApiClient Api => Shell.Api;

    /// <summary>Fuer XAML-Bindings, die den App-Zustand brauchen (z.B. Domain-Liste).</summary>
    public MainViewModel Host => Shell;

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string? _statusMessage;
    [ObservableProperty] private bool _statusIsError;

    /// <summary>Muss diese Seite eine ausgewaehlte Domain haben, um zu funktionieren?</summary>
    public virtual bool RequiresDomain => false;

    protected PageViewModelBase(MainViewModel shell) => Shell = shell;

    /// <summary>Wird beim Navigieren zur Seite und bei Domain-Wechsel aufgerufen.</summary>
    public virtual Task RefreshAsync() => Task.CompletedTask;

    protected string? CurrentDomain => Shell.SelectedDomain;

    /// <summary>Fuehrt eine API-Operation mit Ladezustand und einheitlicher Fehlerbehandlung aus.</summary>
    protected async Task<bool> RunAsync(Func<Task> action, string? successMessage = null)
    {
        if (IsBusy) return false;
        IsBusy = true;
        StatusMessage = null;
        StatusIsError = false;
        try
        {
            await action().ConfigureAwait(true);
            if (successMessage is not null)
            {
                StatusMessage = successMessage;
                StatusIsError = false;
            }
            return true;
        }
        catch (ApiException ex)
        {
            StatusMessage = ex.Field is null ? ex.Message : Localization.Loc.T("Common_FieldSuffix", ex.Message, ex.Field);
            StatusIsError = true;
            return false;
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            StatusIsError = true;
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
