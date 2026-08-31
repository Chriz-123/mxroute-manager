using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Localization;
using MxRouteManager.Models;

namespace MxRouteManager.ViewModels;

public sealed partial class ForwardersViewModel : PageViewModelBase
{
    public override bool RequiresDomain => true;

    public ObservableCollection<Forwarder> Forwarders { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private Forwarder? _selected;

    public bool HasSelection => Selected is not null;

    public ForwardersViewModel(MainViewModel shell) : base(shell) { }

    public override async Task RefreshAsync()
    {
        Forwarders.Clear();
        if (string.IsNullOrEmpty(CurrentDomain)) return;

        await RunAsync(async () =>
        {
            var list = await Api.GetForwardersAsync(CurrentDomain!);
            foreach (var f in list.OrderBy(x => x.Alias, StringComparer.OrdinalIgnoreCase))
                Forwarders.Add(f);
        });
    }

    [RelayCommand]
    private async Task Create()
    {
        if (string.IsNullOrEmpty(CurrentDomain)) return;
        var input = Shell.Dialogs.PromptForwarder(CurrentDomain!);
        if (input is null) return;

        var ok = await RunAsync(
            async () => await Api.CreateForwarderAsync(CurrentDomain!, input.Alias, input.Destinations),
            Loc.T("Fwd_Created", $"{input.Alias}@{CurrentDomain}"));
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (Selected is null || string.IsNullOrEmpty(CurrentDomain)) return;
        string alias = Selected.Alias;
        if (!Shell.Dialogs.Confirm(Loc.T("Fwd_DeleteTitle"),
                Loc.T("Fwd_DeleteConfirm", $"{alias}@{CurrentDomain}")))
            return;

        var ok = await RunAsync(
            async () => await Api.DeleteForwarderAsync(CurrentDomain!, alias),
            Loc.T("Fwd_Deleted", $"{alias}@{CurrentDomain}"));
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private Task Refresh() => RefreshAsync();
}
