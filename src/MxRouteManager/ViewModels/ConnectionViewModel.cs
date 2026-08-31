using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Services;

namespace MxRouteManager.ViewModels;

public sealed partial class ConnectionViewModel : PageViewModelBase
{
    [ObservableProperty] private string _server = "";
    [ObservableProperty] private string _username = "";
    [ObservableProperty] private string _apiKey = "";
    [ObservableProperty] private bool _rememberCredentials = true;

    public ConnectionViewModel(MainViewModel shell) : base(shell) { }

    [RelayCommand]
    public async Task ConnectAsync()
    {
        if (string.IsNullOrWhiteSpace(Server) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(ApiKey))
        {
            StatusMessage = "Bitte Server, Benutzername und API-Key ausfuellen.";
            StatusIsError = true;
            return;
        }

        Api.SetCredentials(Server, Username, ApiKey);

        var ok = await RunAsync(async () =>
        {
            await Api.VerifyConnectionAsync();
        });

        if (!ok) return;

        // Zugangsdaten speichern bzw. loeschen
        if (RememberCredentials)
        {
            Shell.Store.Save(new AppSettings
            {
                Server = Server.Trim(),
                Username = Username.Trim(),
                ApiKeyProtected = Shell.Store.Protect(ApiKey.Trim()),
                RememberCredentials = true
            });
        }
        else
        {
            Shell.Store.Clear();
        }

        StatusMessage = "Verbindung erfolgreich.";
        StatusIsError = false;
        await Shell.OnConnectedAsync();
    }
}
