using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Localization;

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
            StatusMessage = Loc.T("Spam_ScoreRange");
            StatusIsError = true;
            return;
        }
        await RunAsync(async () => await Api.UpdateSpamSettingsAsync(CurrentDomain!, HighScore),
            Loc.T("Spam_ScoreSaved", HighScore));
    }

    [RelayCommand]
    private async Task AddWhitelist()
    {
        if (string.IsNullOrEmpty(CurrentDomain)) return;
        var entry = Shell.Dialogs.PromptText(Loc.T("Spam_WhitelistTitle"), Loc.T("Spam_ListPrompt"),
            hint: Loc.T("Spam_WhitelistHint"));
        if (string.IsNullOrWhiteSpace(entry)) return;

        var ok = await RunAsync(async () => await Api.AddWhitelistAsync(CurrentDomain!, entry.Trim()),
            Loc.T("Spam_WhitelistAdded"));
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private async Task RemoveWhitelist()
    {
        if (SelectedWhitelist is null || string.IsNullOrEmpty(CurrentDomain)) return;
        var entry = SelectedWhitelist;
        var ok = await RunAsync(async () => await Api.RemoveWhitelistAsync(CurrentDomain!, entry),
            Loc.T("Spam_WhitelistRemoved"));
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private async Task AddBlacklist()
    {
        if (string.IsNullOrEmpty(CurrentDomain)) return;
        var entry = Shell.Dialogs.PromptText(Loc.T("Spam_BlacklistTitle"), Loc.T("Spam_ListPrompt"),
            hint: Loc.T("Spam_BlacklistHint"));
        if (string.IsNullOrWhiteSpace(entry)) return;

        var ok = await RunAsync(async () => await Api.AddBlacklistAsync(CurrentDomain!, entry.Trim()),
            Loc.T("Spam_BlacklistAdded"));
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private async Task RemoveBlacklist()
    {
        if (SelectedBlacklist is null || string.IsNullOrEmpty(CurrentDomain)) return;
        var entry = SelectedBlacklist;
        var ok = await RunAsync(async () => await Api.RemoveBlacklistAsync(CurrentDomain!, entry),
            Loc.T("Spam_BlacklistRemoved"));
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private Task Refresh() => RefreshAsync();
}
