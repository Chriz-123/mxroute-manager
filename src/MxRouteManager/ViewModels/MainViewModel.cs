using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Localization;
using MxRouteManager.Services;

namespace MxRouteManager.ViewModels;

/// <summary>Ein Eintrag der Sidebar-Navigation (Titel folgt der aktuellen Sprache).</summary>
public sealed partial class NavItem : ObservableObject
{
    public string Key { get; }
    public string TitleKey { get; }
    public string Icon { get; }
    public bool RequiresDomain { get; }

    public string Title => Loc.Instance[TitleKey];

    public NavItem(string key, string titleKey, string icon, bool requiresDomain)
    {
        Key = key;
        TitleKey = titleKey;
        Icon = icon;
        RequiresDomain = requiresDomain;
        Loc.Instance.LanguageChanged += () => OnPropertyChanged(nameof(Title));
    }
}

public sealed partial class MainViewModel : ObservableObject
{
    public MxRouteApiClient Api { get; } = new();
    public DialogService Dialogs { get; } = new();
    public SettingsStore Store { get; } = new();

    private readonly Dictionary<string, PageViewModelBase> _pages = new();

    [ObservableProperty] private bool _isConnected;
    [ObservableProperty] private string _connectionLabel = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDomains))]
    private string? _selectedDomain;

    [ObservableProperty] private PageViewModelBase? _currentPage;
    [ObservableProperty] private string _currentNavKey = "connection";
    [ObservableProperty] private bool _isLoadingDomains;
    [ObservableProperty] private string _currentLanguage = "de";

    public ObservableCollection<string> Domains { get; } = new();
    public ObservableCollection<NavItem> NavItems { get; } = new();

    public bool HasDomains => Domains.Count > 0;

    public ConnectionViewModel Connection { get; }

    public MainViewModel()
    {
        // Sprache aus den Einstellungen laden, bevor die UI aufgebaut wird.
        var initial = Store.Load();
        Loc.Instance.SetLanguage(initial.Language);
        CurrentLanguage = Loc.Instance.Language;

        NavItems.Add(new NavItem("dashboard", "Nav_Dashboard", "\uD83D\uDCCA", false));
        NavItems.Add(new NavItem("domains", "Nav_Domains", "\uD83C\uDF10", false));
        NavItems.Add(new NavItem("email", "Nav_Email", "\u2709", true));
        NavItems.Add(new NavItem("forwarders", "Nav_Forwarders", "\u21AA", true));
        NavItems.Add(new NavItem("pointers", "Nav_Pointers", "\uD83D\uDD17", true));
        NavItems.Add(new NavItem("spam", "Nav_Spam", "\uD83D\uDEE1", true));
        NavItems.Add(new NavItem("catchall", "Nav_CatchAll", "\uD83D\uDCE5", true));
        NavItems.Add(new NavItem("dns", "Nav_Dns", "\uD83D\uDDC2", true));
        NavItems.Add(new NavItem("verify", "Nav_Verify", "\uD83D\uDD11", false));

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
        ConnectionLabel = Loc.T("Sidebar_NotConnected");

        Domains.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasDomains));
        Loc.Instance.LanguageChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        if (!IsConnected)
            ConnectionLabel = Loc.T("Sidebar_NotConnected");
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
    private void SetLanguage(string language)
    {
        Loc.Instance.SetLanguage(language);
        CurrentLanguage = Loc.Instance.Language;

        // Sprache dauerhaft speichern (Zugangsdaten bleiben erhalten).
        var s = Store.Load();
        s.Language = Loc.Instance.Language;
        Store.Save(s);
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
        ConnectionLabel = Loc.T("Sidebar_NotConnected");
        Domains.Clear();
        SelectedDomain = null;
        CurrentNavKey = "connection";
        CurrentPage = Connection;
        _ = Connection.RefreshAsync();
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
            Dialogs.Error(Loc.T("Domains_LoadTitle"), ex.Message);
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
