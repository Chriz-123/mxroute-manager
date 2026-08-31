# MXroute Manager

Eine native Windows-Desktop-Anwendung (WPF, .NET 8) zur Verwaltung eines
[MXroute](https://mxroute.com)-E-Mail-Hostings über die offizielle
[MXroute API](https://api.mxroute.com/docs).

Es sind **alle Nicht-Reseller-Funktionen** der API integriert. Reseller-Funktionen
(Benutzer- und Paketverwaltung) sind bewusst ausgelassen.

> **Hinweis:** Dies ist ein **inoffizieller** Client. Das Projekt steht in keiner
> Verbindung zu MXroute und wird nicht von MXroute unterstützt. „MXroute“ ist eine
> Marke der jeweiligen Inhaber und wird hier nur zur Beschreibung der Kompatibilität
> verwendet.

## Funktionen

| Bereich | Funktionen |
|---------|------------|
| **Dashboard** | Gesamt-Speichernutzung, Aufschlüsselung (E-Mail/Web/DB/Backups/Sonstiges), Speicher pro Postfach, Kulanzfrist-Warnung |
| **Domains** | Auflisten, Hinzufügen, Details ansehen, Löschen, Mail-Hosting an/aus |
| **E-Mail-Konten** | Auflisten, Anlegen (mit Passwort-Generator), Bearbeiten (Passwort/Quota/Limit), Löschen |
| **Weiterleitungen** | Auflisten, Anlegen (mehrere Ziele, `:blackhole:` / `:fail:`), Löschen |
| **Domain-Pointer** | Auflisten, Anlegen (Alias oder Weiterleitung), Löschen |
| **Spam-Filter** | Score-Schwelle setzen, Whitelist & Blacklist verwalten |
| **Catch-All** | `fail` / `blackhole` / Weiterleitung an Adresse |
| **DNS-Info** | MX-, SPF-, DKIM- und Verifizierungs-Einträge (nur Anzeige, kopierbar) |
| **Verifizierungs-Key** | TXT-Eintrag zum Hinzufügen neuer Domains abrufen und kopieren |

## Zugangsdaten

Die App benötigt die drei API-Header, die im
[mxpanel](https://panel.mxroute.com/api-keys.php) unter *API Keys* zu finden sind:

- **X-Server** – Mailserver-Hostname (z. B. `eagle.mxlogin.com`)
- **X-Username** – DirectAdmin-Benutzername
- **X-API-Key** – der erstellte API-Key

Auf Wunsch werden die Zugangsdaten lokal gespeichert. Der API-Key wird dabei mit der
Windows-DPAPI (nur für den angemeldeten Windows-Benutzer entschlüsselbar) verschlüsselt
unter `%APPDATA%\MxRouteManager\settings.json` abgelegt.

## Voraussetzungen (zur Ausführung)

Die App wird **framework-abhängig** ausgeliefert – die `.exe` ist dadurch nur ~0,5 MB groß,
setzt aber eine installierte Runtime voraus:

- **Windows x64** (Windows 10 / 11)
- **[.NET 8 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0)**
  – der Eintrag **„.NET Desktop Runtime 8.x“** (nicht nur „.NET Runtime“, WPF benötigt die Desktop-Variante)

Prüfen, ob die Runtime vorhanden ist:

```powershell
dotnet --list-runtimes
```

In der Ausgabe muss eine Zeile mit `Microsoft.WindowsDesktop.App 8.x` erscheinen.
Fehlt sie, die Desktop Runtime über den obigen Link (oder `winget install Microsoft.DotNet.DesktopRuntime.8`)
installieren.

## Build

Voraussetzung zum Bauen: [.NET 8 SDK](https://dotnet.microsoft.com/download).

```powershell
# Entwicklungs-Build
dotnet build src/MxRouteManager/MxRouteManager.csproj -c Debug

# Schlanke EXE (Standard, framework-abhängig, ~0,5 MB) -> benötigt .NET 8 Desktop Runtime
dotnet publish src/MxRouteManager/MxRouteManager.csproj -c Release -o publish
```

Ergebnis: `publish/MxRouteManager.exe` – eine einzelne, portable `.exe` für Windows x64.

### Alternative: eigenständige EXE ohne Runtime-Abhängigkeit

Wer die App ohne installierte Runtime weitergeben möchte, kann eine self-contained
Variante bauen (größer, ~60 MB, enthält die komplette .NET-Runtime):

```powershell
dotnet publish src/MxRouteManager/MxRouteManager.csproj -c Release -o publish-standalone `
  --self-contained true -p:EnableCompressionInSingleFile=true
```

## Technik

- **WPF / .NET 8** (`net8.0-windows`)
- **MVVM** mit [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
- `HttpClient` + `System.Text.Json` für die API-Anbindung
- DPAPI (`System.Security.Cryptography.ProtectedData`) für die Schlüssel-Verschlüsselung

## Projektstruktur

```
src/MxRouteManager/
├── Models/           Datenmodelle (DTOs) der API
├── Services/         API-Client, Einstellungs-Speicher, Dialog-Service
├── ViewModels/       MVVM-ViewModels (eine pro Seite)
├── Views/            XAML-Ansichten (UserControls)
├── Dialogs/          Eingabe-Dialoge (Konto, Weiterleitung, Pointer, Text)
├── Converters/       WPF-Value-Converter
├── Themes/           Dark-Theme (Farben & Control-Styles)
├── MainWindow.xaml   Shell mit Sidebar-Navigation
└── App.xaml          Ressourcen & VM→View-Zuordnung

docs/openapi.yaml     Referenz: OpenAPI-Spezifikation der MXroute API
```

## Rate-Limits

Die API begrenzt Anfragen (100 Lese- bzw. 20 Schreibvorgänge pro Minute). Bei
Überschreitung meldet die App den Fehler `RATE_LIMITED`; kurz warten und erneut versuchen.

## Lizenz

Dieses Projekt steht unter der [MIT-Lizenz](LICENSE) – frei nutzbar, veränderbar und
weitergebbar, ohne Gewährleistung.

Verwendete Bibliotheken stehen ebenfalls unter der MIT-Lizenz
(.NET / WPF, CommunityToolkit.Mvvm, System.Security.Cryptography.ProtectedData).
