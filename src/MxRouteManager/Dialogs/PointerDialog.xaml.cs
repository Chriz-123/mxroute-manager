using System.Windows;

namespace MxRouteManager.Dialogs;

public partial class PointerDialog : Window
{
    public string Pointer => PointerBox.Text.Trim();
    public bool IsAlias => AliasRadio.IsChecked == true;

    public PointerDialog()
    {
        InitializeComponent();
        Loaded += (_, _) => PointerBox.Focus();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PointerBox.Text)) return;
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
