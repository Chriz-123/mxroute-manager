using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Localization;

namespace MxRouteManager.ViewModels;

public sealed partial class CatchAllViewModel : PageViewModelBase
{
    public override bool RequiresDomain => true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAddressType))]
    private string _selectedType = "fail";

    [ObservableProperty] private string _address = "";
    [ObservableProperty] private string _currentDescription = "";

    public bool IsAddressType => SelectedType == "address";

    public CatchAllViewModel(MainViewModel shell) : base(shell) { }

    public override async Task RefreshAsync()
    {
        if (string.IsNullOrEmpty(CurrentDomain)) return;

        await RunAsync(async () =>
        {
            var ca = await Api.GetCatchAllAsync(CurrentDomain!);
            if (ca is not null)
            {
                SelectedType = ca.Type;
                Address = ca.Address ?? "";
                CurrentDescription = ca.Description ?? "";
            }
        });
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrEmpty(CurrentDomain)) return;
        if (IsAddressType && string.IsNullOrWhiteSpace(Address))
        {
            StatusMessage = Loc.T("CatchAll_NeedAddress");
            StatusIsError = true;
            return;
        }

        var ok = await RunAsync(
            async () => await Api.SetCatchAllAsync(CurrentDomain!, SelectedType, IsAddressType ? Address.Trim() : null),
            Loc.T("CatchAll_Saved"));
        if (ok) await RefreshAsync();
    }

    [RelayCommand]
    private Task Refresh() => RefreshAsync();
}
