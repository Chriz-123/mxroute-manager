using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MxRouteManager.ViewModels;

public sealed partial class SpamViewModel : PageViewModelBase
{
    public override bool RequiresDomain => true;

    [ObservableProperty] private int _highScore = 5;

    [ObservableProperty] private string? _selectedWhitelist;
    [ObservableProperty] private string? _selectedBlacklist;

    public ObservableCollection<string> Whitelist { get; } = new();
    public ObservableCollection<string> Blacklist { get; } = new();

    public SpamViewModel(MainViewModel shell) : base(shell) { }

    public override async Task RefreshAsync()
    {
        Whitelist.Clear();
        Blacklist.Clear();
        if (string.IsNullOrEmpty(CurrentDomain)) return;

        await RunAsync(async () =>
        {
            var settings = await Api.GetSpamSettingsAsync(CurrentDomain!);
            if (settings is not null) HighScore = settings.HighScore;

            foreach (var w in await Api.GetWhitelistAsync(CurrentDomain!)) Whitelist.Add(w);
            foreach (var b in await Api.GetBlacklistAsync(CurrentDomain!)) Blacklist.Add(b);
        });
    }

    [RelayCommand]
    private async Task SaveSettings()
    {
        if (string.IsNullOrEmpty(CurrentDomain)) return;
        if (HighScore < 1 || HighScore > 50)
        {
            StatusMessage = "Score muss zwischen 1 und 50 liegen.";
            StatusIsError = true;
            return;
        }
        await RunAsync(async () => await Api.UpdateSpamSettingsAsync(CurrentDomain!, HighScore),
            $"Spam-Score auf {HighScore} gesetzt.");
    }

    [RelayCommand]
    private async Task AddWhitelist()
    {
        if (string.IsNullOrEmpty(CurrentDomain)) return;
        var entry = Shell.Dialogs.PromptText("Whitelist-Eintrag", "E-Mail-Adresse oder Muster (Wildcards erlaubt):",
            hint: "Beispiel: *@vertrauenswuerdig.de");
        if (string.IsNullOrWhiteSpace(entry)) return;

        var ok = await RunAsync(async () => await Api.AddWhitelistAsync(CurrentDomain!, entry.Trim()),
            "Eintrag zur Whitelist hinzugefuegt.");
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private async Task RemoveWhitelist()
    {
        if (SelectedWhitelist is null || string.IsNullOrEmpty(CurrentDomain)) return;
        var entry = SelectedWhitelist;
        var ok = await RunAsync(async () => await Api.RemoveWhitelistAsync(CurrentDomain!, entry),
            "Whitelist-Eintrag entfernt.");
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private async Task AddBlacklist()
    {
        if (string.IsNullOrEmpty(CurrentDomain)) return;
        var entry = Shell.Dialogs.PromptText("Blacklist-Eintrag", "E-Mail-Adresse oder Muster (Wildcards erlaubt):",
            hint: "Beispiel: *@spam-quelle.de");
        if (string.IsNullOrWhiteSpace(entry)) return;

        var ok = await RunAsync(async () => await Api.AddBlacklistAsync(CurrentDomain!, entry.Trim()),
            "Eintrag zur Blacklist hinzugefuegt.");
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private async Task RemoveBlacklist()
    {
        if (SelectedBlacklist is null || string.IsNullOrEmpty(CurrentDomain)) return;
        var entry = SelectedBlacklist;
        var ok = await RunAsync(async () => await Api.RemoveBlacklistAsync(CurrentDomain!, entry),
            "Blacklist-Eintrag entfernt.");
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private Task Refresh() => RefreshAsync();
}
