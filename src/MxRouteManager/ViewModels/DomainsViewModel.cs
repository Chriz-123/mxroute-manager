using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            "Domain hinzufuegen",
            "Name der Domain, die dem Konto hinzugefuegt werden soll:",
            hint: "Hinweis: Vor dem Hinzufuegen muss der Domain-Verifizierungs-TXT-Eintrag gesetzt sein (siehe 'Verifizierungs-Key').");
        if (string.IsNullOrWhiteSpace(name)) return;

        var ok = await RunAsync(async () => await Api.CreateDomainAsync(name.Trim()),
            $"Domain '{name.Trim()}' hinzugefuegt.");
        if (ok) await Shell.ReloadDomainsAsync(name.Trim());
        if (ok) { SelectedName = name.Trim(); }
    }

    [RelayCommand]
    private async Task DeleteDomain()
    {
        if (string.IsNullOrEmpty(SelectedName)) return;
        if (!Shell.Dialogs.Confirm("Domain loeschen",
                $"Domain '{SelectedName}' wirklich vom Konto entfernen? Alle zugehoerigen E-Mail-Daten gehen verloren."))
            return;

        var name = SelectedName;
        var ok = await RunAsync(async () => await Api.DeleteDomainAsync(name!),
            $"Domain '{name}' geloescht.");
        if (ok) await Shell.ReloadDomainsAsync();
    }

    [RelayCommand]
    private async Task ApplyMailHosting()
    {
        if (string.IsNullOrEmpty(SelectedName)) return;
        var name = SelectedName;
        var enabled = DetailsMailHosting;
        await RunAsync(async () => await Api.SetMailStatusAsync(name!, enabled),
            $"Mail-Hosting fuer '{name}' {(enabled ? "aktiviert" : "deaktiviert")}.");
    }

    [RelayCommand]
    private Task Refresh() => RefreshAsync();
}
