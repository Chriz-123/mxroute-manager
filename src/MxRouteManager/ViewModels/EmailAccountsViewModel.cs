using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Localization;
using MxRouteManager.Models;

namespace MxRouteManager.ViewModels;

public sealed partial class EmailAccountsViewModel : PageViewModelBase
{
    public override bool RequiresDomain => true;

    public ObservableCollection<EmailAccount> Accounts { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private EmailAccount? _selected;

    public bool HasSelection => Selected is not null;

    public EmailAccountsViewModel(MainViewModel shell) : base(shell) { }

    public override async Task RefreshAsync()
    {
        Accounts.Clear();
        if (string.IsNullOrEmpty(CurrentDomain)) return;

        await RunAsync(async () =>
        {
            var list = await Api.GetEmailAccountsAsync(CurrentDomain!);
            foreach (var a in list.OrderBy(x => x.Username, StringComparer.OrdinalIgnoreCase))
                Accounts.Add(a);
        });
    }

    [RelayCommand]
    private async Task Create()
    {
        if (string.IsNullOrEmpty(CurrentDomain)) return;
        var input = Shell.Dialogs.PromptCreateEmailAccount(CurrentDomain!);
        if (input is null) return;

        var ok = await RunAsync(
            async () => await Api.CreateEmailAccountAsync(CurrentDomain!, input.Username, input.Password, input.Quota, input.Limit),
            Loc.T("Email_Created", $"{input.Username}@{CurrentDomain}"));
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private async Task Edit()
    {
        if (Selected is null || string.IsNullOrEmpty(CurrentDomain)) return;
        var input = Shell.Dialogs.PromptEditEmailAccount(CurrentDomain!, Selected);
        if (input is null) return;

        string user = Selected.Username;
        var ok = await RunAsync(
            async () => await Api.UpdateEmailAccountAsync(
                CurrentDomain!, user,
                string.IsNullOrEmpty(input.Password) ? null : input.Password,
                input.Quota, input.Limit),
            Loc.T("Email_Updated", $"{user}@{CurrentDomain}"));
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (Selected is null || string.IsNullOrEmpty(CurrentDomain)) return;
        string user = Selected.Username;
        if (!Shell.Dialogs.Confirm(Loc.T("Email_DeleteTitle"),
                Loc.T("Email_DeleteConfirm", $"{user}@{CurrentDomain}")))
            return;

        var ok = await RunAsync(
            async () => await Api.DeleteEmailAccountAsync(CurrentDomain!, user),
            Loc.T("Email_Deleted", $"{user}@{CurrentDomain}"));
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private Task Refresh() => RefreshAsync();
}
