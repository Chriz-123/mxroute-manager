using System.Windows;
using MxRouteManager.Dialogs;
using MxRouteManager.Models;

namespace MxRouteManager.Services;

public sealed record EmailAccountInput(string Username, string Password, int Quota, int Limit);
public sealed record ForwarderInput(string Alias, List<string> Destinations);
public sealed record PointerInput(string Pointer, bool Alias);

/// <summary>Kapselt WPF-Dialoge, damit ViewModels UI-frei bleiben.</summary>
public sealed class DialogService
{
    private static Window? Owner => Application.Current?.MainWindow;

    public bool Confirm(string title, string message)
        => MessageBox.Show(Owner!, message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)
           == MessageBoxResult.Yes;

    public void Info(string title, string message)
        => MessageBox.Show(Owner!, message, title, MessageBoxButton.OK, MessageBoxImage.Information);

    public void Error(string title, string message)
        => MessageBox.Show(Owner!, message, title, MessageBoxButton.OK, MessageBoxImage.Error);

    public string? PromptText(string title, string prompt, string initial = "", string? hint = null)
    {
        var dlg = new TextInputDialog(title, prompt, initial, hint) { Owner = Owner };
        return dlg.ShowDialog() == true ? dlg.Value : null;
    }

    public PointerInput? PromptPointer()
    {
        var dlg = new PointerDialog { Owner = Owner };
        return dlg.ShowDialog() == true ? new PointerInput(dlg.Pointer, dlg.IsAlias) : null;
    }

    public EmailAccountInput? PromptCreateEmailAccount(string domain)
    {
        var dlg = new EmailAccountDialog(domain) { Owner = Owner };
        return dlg.ShowDialog() == true
            ? new EmailAccountInput(dlg.Username, dlg.Password, dlg.Quota, dlg.Limit)
            : null;
    }

    public EmailAccountInput? PromptEditEmailAccount(string domain, EmailAccount account)
    {
        var dlg = new EmailAccountDialog(domain, account) { Owner = Owner };
        return dlg.ShowDialog() == true
            ? new EmailAccountInput(dlg.Username, dlg.Password, dlg.Quota, dlg.Limit)
            : null;
    }

    public ForwarderInput? PromptForwarder(string domain)
    {
        var dlg = new ForwarderDialog(domain) { Owner = Owner };
        return dlg.ShowDialog() == true ? new ForwarderInput(dlg.Alias, dlg.Destinations) : null;
    }
}
