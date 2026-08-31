using System.Windows;

namespace MxRouteManager.Dialogs;

public partial class ForwarderDialog : Window
{
    public string Alias => AliasBox.Text.Trim();
    public List<string> Destinations { get; private set; } = new();

    public ForwarderDialog(string domain)
    {
        InitializeComponent();
        DomainSuffix.Text = "@" + domain;
        Loaded += (_, _) => AliasBox.Focus();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Visibility = Visibility.Collapsed;

        if (string.IsNullOrWhiteSpace(Alias))
        {
            ShowError("Bitte einen Alias eingeben.");
            return;
        }

        var dests = DestinationsBox.Text
            .Split(new[] { '\r', '\n', ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct()
            .ToList();

        if (dests.Count == 0)
        {
            ShowError("Bitte mindestens ein Ziel angeben.");
            return;
        }

        Destinations = dests;
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void ShowError(string msg)
    {
        ErrorText.Text = msg;
        ErrorText.Visibility = Visibility.Visible;
    }
}
