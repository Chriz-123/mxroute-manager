using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Services;

namespace MxRouteManager.ViewModels;

public sealed record NavItem(string Key, string Title, string Icon, bool RequiresDomain);

public sealed partial class MainViewModel : ObservableObject
{
    public MxRouteApiClient Api { get; } = new();
    public DialogService Dialogs { get; } = new();
    public SettingsStore Store { get; } = new();

    private readonly Dictionary<string, PageViewModelBase> _pages = new();

    [ObservableProperty] private bool _isConnected;
    [ObservableProperty] private string _connectionLabel = "Nicht verbunden";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDomains))]
    private string? _selectedDomain;

    [ObservableProperty] private PageViewModelBase? _currentPage;
    [ObservableProperty] private string _currentNavKey = "connection";
    [ObservableProperty] private bool _isLoadingDomains;

    public ObservableCollection<string> Domains { get; } = new();
    public ObservableCollection<NavItem> NavItems { get; } = new();

    public bool HasDomains => Domains.Count > 0;

    public ConnectionViewModel Connection { get; }

    public MainViewModel()
    {
        NavItems.Add(new NavItem("dashboard", "Dashboard", "\uD83D\uDCCA", false));
        NavItems.Add(new NavItem("domains", "Domains", "\uD83C\uDF10", false));
        NavItems.Add(new NavItem("email", "E-Mail-Konten", "\u2709", true));
        NavItems.Add(new NavItem("forwarders", "Weiterleitungen", "\u21AA", true));
        NavItems.Add(new NavItem("pointers", "Domain-Pointer", "\uD83D\uDD17", true));
        NavItems.Add(new NavItem("spam", "Spam-Filter", "\uD83D\uDEE1", true));
        NavItems.Add(new NavItem("catchall", "Catch-All", "\uD83D\uDCE5", true));
        NavItems.Add(new NavItem("dns", "DNS-Info", "\uD83D\uDDC2", true));
        NavItems.Add(new NavItem("verify", "Verifizierungs-Key", "\uD83D\uDD11", false));

        Connection = new ConnectionViewModel(this);

        _pages["connection"] = Connection;
        _pages["dashboard"] = new DashboardViewModel(this);
        _pages["domains"] = new DomainsViewModel(this);
        _pages["email"] = new EmailAccountsViewModel(this);
        _pages["forwarders"] = new ForwardersViewModel(this);
        _pages["pointers"] = new PointersViewModel(this);
        _pages["spam"] = new SpamViewModel(this);
        _pages["catchall"] = new CatchAllViewModel(this);
        _pages["dns"] = new DnsViewModel(this);
        _pages["verify"] = new VerificationKeyViewModel(this);

        CurrentPage = Connection;

        Domains.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasDomains));
    }

    /// <summary>Beim Start: gespeicherte Zugangsdaten laden und ggf. automatisch verbinden.</summary>
    public async Task InitializeAsync()
    {
        var s = Store.Load();
        Connection.Server = s.Server;
        Connection.Username = s.Username;
        Connection.RememberCredentials = s.RememberCredentials;

        var key = Store.Unprotect(s.ApiKeyProtected);
        Connection.ApiKey = key;

        if (s.RememberCredentials && !string.IsNullOrEmpty(s.Server)
            && !string.IsNullOrEmpty(s.Username) && !string.IsNullOrEmpty(key))
        {
            await Connection.ConnectAsync();
        }
    }

    [RelayCommand]
    private async Task Navigate(string key)
    {
        if (!_pages.TryGetValue(key, out var page)) return;
        if (page.RequiresDomain && string.IsNullOrEmpty(SelectedDomain)) return;

        CurrentNavKey = key;
        CurrentPage = page;
        await page.RefreshAsync();
    }

    /// <summary>Nach erfolgreicher Verbindung aufgerufen.</summary>
    public async Task OnConnectedAsync()
    {
        IsConnected = true;
        ConnectionLabel = $"{Api.Username} @ {Api.Server}";
        await ReloadDomainsAsync();
        await Navigate("dashboard");
    }

    [RelayCommand]
    private void Disconnect()
    {
        IsConnected = false;
        ConnectionLabel = "Nicht verbunden";
        Domains.Clear();
        SelectedDomain = null;
        CurrentNavKey = "connection";
        CurrentPage = Connection;
    }

    [RelayCommand]
    private async Task ReloadDomains()
    {
        await ReloadDomainsAsync();
    }

    public async Task ReloadDomainsAsync(string? selectAfter = null)
    {
        if (string.IsNullOrEmpty(Api.Server)) return;
        IsLoadingDomains = true;
        try
        {
            var current = SelectedDomain;
            var list = await Api.GetDomainsAsync();
            Domains.Clear();
            foreach (var d in list.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                Domains.Add(d);

            var target = selectAfter ?? current;
            if (target is not null && Domains.Contains(target))
                SelectedDomain = target;
            else if (Domains.Count > 0)
                SelectedDomain = Domains[0];
            else
                SelectedDomain = null;
        }
        catch (ApiException ex)
        {
            Dialogs.Error("Domains laden", ex.Message);
        }
        finally
        {
            IsLoadingDomains = false;
        }
    }

    partial void OnSelectedDomainChanged(string? value)
    {
        if (CurrentPage is { RequiresDomain: true } page && !string.IsNullOrEmpty(value))
        {
            _ = page.RefreshAsync();
        }
    }
}
