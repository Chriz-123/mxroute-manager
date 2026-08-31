using System.ComponentModel;

namespace MxRouteManager.Localization;

/// <summary>
/// Zentrale Lokalisierung mit Laufzeit-Umschaltung. XAML bindet ueber den Indexer
/// (<c>{loc:Tr Key}</c>); bei Sprachwechsel werden alle Bindings aktualisiert.
/// </summary>
public sealed class Loc : INotifyPropertyChanged
{
    public static Loc Instance { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Wird nach jedem Sprachwechsel ausgeloest.</summary>
    public event Action? LanguageChanged;

    private string _language = "de";

    private Loc() { }

    public string Language => _language;

    public string this[string key] => Strings.Get(key, _language);

    public void SetLanguage(string language)
    {
        language = language == "en" ? "en" : "de";
        if (_language == language) return;
        _language = language;
        // Alle Indexer-Bindings aktualisieren.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        LanguageChanged?.Invoke();
    }

    /// <summary>Uebersetzt einen Schluessel, optional mit String.Format-Argumenten.</summary>
    public static string T(string key, params object?[] args)
    {
        var s = Strings.Get(key, Instance._language);
        return args.Length == 0 ? s : string.Format(s, args);
    }
}
