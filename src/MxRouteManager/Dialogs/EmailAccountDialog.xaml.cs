using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using MxRouteManager.Models;

namespace MxRouteManager.Dialogs;

public partial class EmailAccountDialog : Window
{
    public bool IsEdit { get; }

    public string Username => UsernameBox.Text.Trim();
    public string Password => PasswordBox.Password;
    public int Quota { get; private set; }
    public int Limit { get; private set; }

    /// <summary>Anlegen-Modus.</summary>
    public EmailAccountDialog(string domain)
    {
        InitializeComponent();
        IsEdit = false;
        DomainSuffix.Text = "@" + domain;
        Title = "E-Mail-Konto anlegen";
        HeaderText.Text = "E-Mail-Konto anlegen";
        OkButton.Content = "Anlegen";
        Loaded += (_, _) => UsernameBox.Focus();
    }

    /// <summary>Bearbeiten-Modus.</summary>
    public EmailAccountDialog(string domain, EmailAccount account)
    {
        InitializeComponent();
        IsEdit = true;
        DomainSuffix.Text = "@" + domain;
        Title = "E-Mail-Konto bearbeiten";
        HeaderText.Text = "E-Mail-Konto bearbeiten";
        OkButton.Content = "Speichern";

        UsernameBox.Text = account.Username;
        UsernameBox.IsEnabled = false;
        PasswordLabel.Text = "Neues Passwort (leer lassen = unveraendert)";
        QuotaBox.Text = account.Quota.ToString();
        LimitBox.Text = account.Limit.ToString();
        Loaded += (_, _) => PasswordBox.Focus();
    }

    private void Generate_Click(object sender, RoutedEventArgs e)
    {
        PasswordBox.Password = GeneratePassword(16);
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Visibility = Visibility.Collapsed;

        if (!IsEdit && string.IsNullOrWhiteSpace(Username))
        {
            ShowError("Bitte einen Benutzernamen eingeben.");
            return;
        }

        bool passwordRequired = !IsEdit;
        if (passwordRequired || !string.IsNullOrEmpty(Password))
        {
            if (!IsValidPassword(Password))
            {
                ShowError("Passwort erfuellt die Anforderungen nicht (min. 8 Zeichen, Gro\u00df-/Kleinbuchstabe + Ziffer).");
                return;
            }
        }

        if (!int.TryParse(QuotaBox.Text.Trim(), out var quota) || quota < 0)
        {
            ShowError("Quota muss eine Zahl >= 0 sein.");
            return;
        }
        if (!int.TryParse(LimitBox.Text.Trim(), out var limit) || limit < 0 || limit > 9600)
        {
            ShowError("Tageslimit muss zwischen 0 und 9600 liegen.");
            return;
        }

        Quota = quota;
        Limit = limit;
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void ShowError(string msg)
    {
        ErrorText.Text = msg;
        ErrorText.Visibility = Visibility.Visible;
    }

    private static bool IsValidPassword(string pw)
        => pw.Length >= 8
           && Regex.IsMatch(pw, "[A-Z]")
           && Regex.IsMatch(pw, "[a-z]")
           && Regex.IsMatch(pw, "[0-9]");

    private static string GeneratePassword(int length)
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnpqrstuvwxyz";
        const string digits = "23456789";
        const string special = "!@#$%*-_";
        string all = upper + lower + digits + special;

        var sb = new StringBuilder();
        sb.Append(Pick(upper));
        sb.Append(Pick(lower));
        sb.Append(Pick(digits));
        sb.Append(Pick(special));
        for (int i = sb.Length; i < length; i++) sb.Append(Pick(all));

        // Mischen
        var chars = sb.ToString().ToCharArray();
        for (int i = chars.Length - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
        return new string(chars);
    }

    private static char Pick(string set) => set[RandomNumberGenerator.GetInt32(set.Length)];
}
