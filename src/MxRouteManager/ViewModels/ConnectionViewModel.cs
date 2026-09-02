using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Localization;
using MxRouteManager.Models;
using MxRouteManager.Services;

namespace MxRouteManager.ViewModels;

public sealed partial class ConnectionViewModel : PageViewModelBase
{
    private readonly MxRouteStatusClient _status = new();
    private bool _serversFromLive;

    [ObservableProperty] private string _server = "";
    [ObservableProperty] private string _username = "";
    [ObservableProperty] private string _apiKey = "";
    [ObservableProperty] private bool _rememberCredentials = true;
    [ObservableProperty] private string _serverListNote = "";

    public ObservableCollection<MailServer> Servers { get; } = new();

    public ConnectionViewModel(MainViewModel shell) : base(shell)
    {
        ReplaceServers(MxRouteStatusClient.FallbackServers, fromLive: false);
        Loc.Instance.LanguageChanged += UpdateServerListNote;
        _ = LoadServersAsync();
    }

    public override Task RefreshAsync() => LoadServersAsync();

    private async Task LoadServersAsync()
    {
        var (list, fromLive) = await _status.GetAccountServersOrFallbackAsync().ConfigureAwait(true);
        ReplaceServers(list, fromLive);
    }

    private void ReplaceServers(IReadOnlyList<MailServer> list, bool fromLive)
    {
        var current = Server;
        Servers.Clear();
        foreach (var s in list)
            Servers.Add(s);
        _serversFromLive = fromLive;
        if (!string.IsNullOrEmpty(current))
            Server = current;
        UpdateServerListNote();
    }

    private void UpdateServerListNote()
    {
        ServerListNote = _serversFromLive
            ? Loc.T("Conn_ServerListLive", Servers.Count)
            : Loc.T("Conn_ServerListFallback", Servers.Count);
    }

    [RelayCommand]
    public async Task ConnectAsync()
    {
        if (string.IsNullOrWhiteSpace(Server) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(ApiKey))
        {
            StatusMessage = Loc.T("Conn_FillAll");
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
                RememberCredentials = true,
                Language = Loc.Instance.Language
            });
        }
        else
        {
            Shell.Store.Clear();
        }

        StatusMessage = Loc.T("Conn_Success");
        StatusIsError = false;
        await Shell.OnConnectedAsync();
    }
}
