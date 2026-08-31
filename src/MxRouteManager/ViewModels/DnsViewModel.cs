using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Models;

namespace MxRouteManager.ViewModels;

public sealed partial class DnsViewModel : PageViewModelBase
{
    public override bool RequiresDomain => true;

    public ObservableCollection<MxRecord> MxRecords { get; } = new();

    [ObservableProperty] private DnsRecord? _spf;
    [ObservableProperty] private DnsRecord? _dkim;
    [ObservableProperty] private DnsRecord? _verification;
    [ObservableProperty] private bool _hasDkim;
    [ObservableProperty] private bool _hasVerification;
    [ObservableProperty] private bool _hasData;

    public DnsViewModel(MainViewModel shell) : base(shell) { }

    public override async Task RefreshAsync()
    {
        MxRecords.Clear();
        Spf = Dkim = Verification = null;
        HasDkim = HasVerification = HasData = false;
        if (string.IsNullOrEmpty(CurrentDomain)) return;

        await RunAsync(async () =>
        {
            var dns = await Api.GetDnsInfoAsync(CurrentDomain!);
            if (dns is null) return;

            foreach (var mx in dns.MxRecords) MxRecords.Add(mx);
            Spf = dns.Spf;
            Dkim = dns.Dkim;
            Verification = dns.Verification;
            HasDkim = dns.Dkim is not null && !string.IsNullOrEmpty(dns.Dkim.Value);
            HasVerification = dns.Verification is not null && !string.IsNullOrEmpty(dns.Verification.Value);
            HasData = true;
        });
    }

    [RelayCommand]
    private Task Refresh() => RefreshAsync();
}
