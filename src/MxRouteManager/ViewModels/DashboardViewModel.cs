using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MxRouteManager.Localization;
using MxRouteManager.Models;

namespace MxRouteManager.ViewModels;

public sealed partial class DashboardViewModel : PageViewModelBase
{
    [ObservableProperty] private QuotaInfo? _quota;
    [ObservableProperty] private string _usageSummary = "";
    [ObservableProperty] private string _percentDisplay = "";
    [ObservableProperty] private double _percentValue;
    [ObservableProperty] private bool _hasGracePeriod;
    [ObservableProperty] private string _gracePeriodText = "";

    [ObservableProperty] private string _emailBreakdown = "";
    [ObservableProperty] private string _webBreakdown = "";
    [ObservableProperty] private string _dbBreakdown = "";
    [ObservableProperty] private string _backupBreakdown = "";
    [ObservableProperty] private string _otherBreakdown = "";
    [ObservableProperty] private string _updatedAt = "";

    public ObservableCollection<EmailQuotaAccount> EmailUsage { get; } = new();

    public DashboardViewModel(MainViewModel shell) : base(shell) { }

    public override async Task RefreshAsync()
    {
        await RunAsync(async () =>
        {
            var q = await Api.GetQuotaAsync();
            Quota = q;
            if (q is not null)
            {
                string limit = q.TotalLimit == 0 ? Loc.T("Common_Unlimited") : FormatBytes.Humanize(q.TotalLimit);
                UsageSummary = Loc.T("Dash_UsageSummary", FormatBytes.Humanize(q.TotalUsed), limit);
                PercentValue = Math.Clamp(q.PercentUsed, 0, 100);
                PercentDisplay = q.TotalLimit == 0 ? "—" : $"{q.PercentUsed:0.#} %";

                if (q.Breakdown is { } b)
                {
                    EmailBreakdown = FormatBytes.Humanize(b.Email);
                    WebBreakdown = FormatBytes.Humanize(b.Web);
                    DbBreakdown = FormatBytes.Humanize(b.Databases);
                    BackupBreakdown = FormatBytes.Humanize(b.Backups);
                    OtherBreakdown = FormatBytes.Humanize(b.Other);
                }

                HasGracePeriod = q.GracePeriod is not null;
                GracePeriodText = q.GracePeriod is { } g
                    ? Loc.T("Dash_GracePeriod", g.DaysRemaining, g.Deadline ?? "")
                    : "";

                UpdatedAt = string.IsNullOrEmpty(q.UpdatedAt) ? "" : Loc.T("Dash_UpdatedAt", q.UpdatedAt);
            }

            var eq = await Api.GetEmailQuotaAsync();
            EmailUsage.Clear();
            if (eq?.Accounts is not null)
                foreach (var a in eq.Accounts)
                    EmailUsage.Add(a);
        });
    }

    [RelayCommand]
    private Task Refresh() => RefreshAsync();
}
