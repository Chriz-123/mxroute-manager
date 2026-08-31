using System.Windows;

namespace MxRouteManager.Dialogs;

public partial class TextInputDialog : Window
{
    public string Value => InputBox.Text.Trim();

    public TextInputDialog(string title, string prompt, string initial = "", string? hint = null)
    {
        InitializeComponent();
        Title = title;
        TitleText.Text = title;
        PromptText.Text = prompt;
        InputBox.Text = initial;
        if (!string.IsNullOrEmpty(hint))
        {
            HintText.Text = hint;
            HintText.Visibility = Visibility.Visible;
        }
        Loaded += (_, _) => { InputBox.Focus(); InputBox.SelectAll(); };
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(InputBox.Text)) return;
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
