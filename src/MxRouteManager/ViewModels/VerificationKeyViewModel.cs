using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Localization;
using MxRouteManager.Models;

namespace MxRouteManager.ViewModels;

public sealed partial class VerificationKeyViewModel : PageViewModelBase
{
    [ObservableProperty] private VerificationKey? _key;
    [ObservableProperty] private string _recordName = "";
    [ObservableProperty] private string _recordType = "";
    [ObservableProperty] private string _recordValue = "";
    [ObservableProperty] private string _description = "";
    [ObservableProperty] private bool _hasKey;

    public VerificationKeyViewModel(MainViewModel shell) : base(shell) { }

    public override async Task RefreshAsync()
    {
        HasKey = false;
        await RunAsync(async () =>
        {
            var k = await Api.GetVerificationKeyAsync();
            Key = k;
            if (k is not null)
            {
                RecordType = k.Record?.Type ?? "TXT";
                RecordName = k.Record?.Name ?? k.Key ?? "";
                RecordValue = k.Record?.Value ?? "domain-verified";
                Description = k.Description ?? "";
                HasKey = true;
            }
        });
    }

    [RelayCommand]
    private void CopyName()
    {
        if (!string.IsNullOrEmpty(RecordName)) TrySetClipboard(RecordName);
    }

    [RelayCommand]
    private void CopyValue()
    {
        if (!string.IsNullOrEmpty(RecordValue)) TrySetClipboard(RecordValue);
    }

    private void TrySetClipboard(string text)
    {
        try
        {
            Clipboard.SetText(text);
            StatusMessage = Loc.T("Verify_Copied");
            StatusIsError = false;
        }
        catch
        {
            StatusMessage = Loc.T("Verify_CopyFail");
            StatusIsError = true;
        }
    }

    [RelayCommand]
    private Task Refresh() => RefreshAsync();
}
