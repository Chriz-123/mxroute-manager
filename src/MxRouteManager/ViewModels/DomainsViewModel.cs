using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Localization;
using MxRouteManager.Models;

namespace MxRouteManager.ViewModels;

public sealed partial class DomainsViewModel : PageViewModelBase
{
    public ObservableCollection<string> Domains => Shell.Domains;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private string? _selectedName;

    [ObservableProperty] private Domain? _details;
    [ObservableProperty] private bool _detailsMailHosting;

    public bool HasSelection => !string.IsNullOrEmpty(SelectedName);

    public DomainsViewModel(MainViewModel shell) : base(shell) { }

    public override async Task RefreshAsync()
    {
        await Shell.ReloadDomainsAsync();
        if (SelectedName is null || !Domains.Contains(SelectedName))
            SelectedName = Domains.FirstOrDefault();
        await LoadDetailsAsync();
    }

    partial void OnSelectedNameChanged(string? value) => _ = LoadDetailsAsync();

    private async Task LoadDetailsAsync()
    {
        Details = null;
        if (string.IsNullOrEmpty(SelectedName)) return;

        await RunAsync(async () =>
        {
            var d = await Api.GetDomainAsync(SelectedName!);
            Details = d;
            if (d is not null) DetailsMailHosting = d.MailHosting;
        });
    }

    [RelayCommand]
    private async Task AddDomain()
    {
        var name = Shell.Dialogs.PromptText(
            Loc.T("Domains_AddTitle"),
            Loc.T("Domains_AddPrompt"),
            hint: Loc.T("Domains_AddHint"));
        if (string.IsNullOrWhiteSpace(name)) return;

        var ok = await RunAsync(async () => await Api.CreateDomainAsync(name.Trim()),
            Loc.T("Domains_Added", name.Trim()));
        if (ok) await Shell.ReloadDomainsAsync(name.Trim());
        if (ok) { SelectedName = name.Trim(); }
    }

    [RelayCommand]
    private async Task DeleteDomain()
    {
        if (string.IsNullOrEmpty(SelectedName)) return;
        if (!Shell.Dialogs.Confirm(Loc.T("Domains_DeleteTitle"),
                Loc.T("Domains_DeleteConfirm", SelectedName)))
            return;

        var name = SelectedName;
        var ok = await RunAsync(async () => await Api.DeleteDomainAsync(name!),
            Loc.T("Domains_Deleted", name));
        if (ok) await Shell.ReloadDomainsAsync();
    }

    [RelayCommand]
    private async Task ApplyMailHosting()
    {
        if (string.IsNullOrEmpty(SelectedName)) return;
        var name = SelectedName;
        var enabled = DetailsMailHosting;
        await RunAsync(async () => await Api.SetMailStatusAsync(name!, enabled),
            Loc.T("Domains_MailHostingSet", name!, Loc.T(enabled ? "Common_Enabled" : "Common_Disabled")));
    }

    [RelayCommand]
    private Task Refresh() => RefreshAsync();
}
