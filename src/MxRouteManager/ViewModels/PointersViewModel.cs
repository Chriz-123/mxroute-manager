using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Models;

namespace MxRouteManager.ViewModels;

public sealed partial class PointersViewModel : PageViewModelBase
{
    public override bool RequiresDomain => true;

    public ObservableCollection<DomainPointer> Pointers { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private DomainPointer? _selected;

    public bool HasSelection => Selected is not null;

    public PointersViewModel(MainViewModel shell) : base(shell) { }

    public override async Task RefreshAsync()
    {
        Pointers.Clear();
        if (string.IsNullOrEmpty(CurrentDomain)) return;

        await RunAsync(async () =>
        {
            var list = await Api.GetPointersAsync(CurrentDomain!);
            foreach (var p in list.OrderBy(x => x.Pointer, StringComparer.OrdinalIgnoreCase))
                Pointers.Add(p);
        });
    }

    [RelayCommand]
    private async Task Create()
    {
        if (string.IsNullOrEmpty(CurrentDomain)) return;
        var input = Shell.Dialogs.PromptPointer();
        if (input is null) return;

        var ok = await RunAsync(
            async () => await Api.CreatePointerAsync(CurrentDomain!, input.Pointer, input.Alias),
            $"Pointer '{input.Pointer}' angelegt.");
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (Selected is null || string.IsNullOrEmpty(CurrentDomain)) return;
        string pointer = Selected.Pointer;
        if (!Shell.Dialogs.Confirm("Pointer loeschen",
                $"Domain-Pointer '{pointer}' wirklich loeschen?"))
            return;

        var ok = await RunAsync(
            async () => await Api.DeletePointerAsync(CurrentDomain!, pointer),
            $"Pointer '{pointer}' geloescht.");
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private Task Refresh() => RefreshAsync();
}
