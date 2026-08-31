using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MxRouteManager.Services;

/// <summary>Auf der Platte gespeicherte Verbindungseinstellungen.</summary>
public sealed class AppSettings
{
    public string Server { get; set; } = "";
    public string Username { get; set; } = "";

    /// <summary>Per DPAPI (aktueller Benutzer) verschluesselter API-Key, Base64-kodiert.</summary>
    public string ApiKeyProtected { get; set; } = "";

    public bool RememberCredentials { get; set; } = true;

    /// <summary>UI-Sprache: "de" oder "en".</summary>
    public string Language { get; set; } = "de";
}

/// <summary>
/// Laedt/speichert Einstellungen unter %APPDATA%\MxRouteManager\settings.json.
/// Der API-Key wird mit der Windows-DPAPI verschluesselt (nur fuer den aktuellen Benutzer entschluesselbar).
/// </summary>
public sealed class SettingsStore
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("MxRouteManager.v1");

    private readonly string _dir;
    private readonly string _file;

    public SettingsStore()
    {
        _dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MxRouteManager");
        _file = Path.Combine(_dir, "settings.json");
    }

    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(_file)) return new AppSettings();
            var json = File.ReadAllText(_file);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public void Save(AppSettings settings)
    {
        Directory.CreateDirectory(_dir);
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_file, json);
    }

    public void Clear()
    {
        try { if (File.Exists(_file)) File.Delete(_file); } catch { /* egal */ }
    }

    public string Protect(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext)) return "";
        var bytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(plaintext), Entropy, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(bytes);
    }

    public string Unprotect(string protectedBase64)
    {
        if (string.IsNullOrEmpty(protectedBase64)) return "";
        try
        {
            var bytes = ProtectedData.Unprotect(Convert.FromBase64String(protectedBase64), Entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return "";
        }
    }
}
