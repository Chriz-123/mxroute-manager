# MXroute Manager

**Sprache / Language:** [Deutsch](#deutsch) · [English](#english)

> **Hinweis / Note:** Inoffizieller Client – nicht mit MXroute verbunden. · Unofficial client – not affiliated with MXroute.

---

<a name="deutsch"></a>

## Deutsch

Eine native Windows-Desktop-Anwendung (WPF, .NET 8) zur Verwaltung eines
[MXroute](https://mxroute.com)-E-Mail-Hostings über die offizielle
[MXroute API](https://api.mxroute.com/docs).

Es sind **alle Nicht-Reseller-Funktionen** der API integriert. Reseller-Funktionen
(Benutzer- und Paketverwaltung) sind bewusst ausgelassen.

Die Oberfläche ist **zweisprachig (Deutsch/Englisch)** und lässt sich jederzeit über
den **DE/EN-Umschalter** in der Seitenleiste umstellen – ohne Neustart. Die Wahl wird
gespeichert.

> Dies ist ein **inoffizieller** Client. Das Projekt steht in keiner Verbindung zu
> MXroute und wird nicht von MXroute unterstützt. „MXroute“ ist eine Marke der
> jeweiligen Inhaber und wird hier nur zur Beschreibung der Kompatibilität verwendet.

### Funktionen

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

### Zugangsdaten

Die App benötigt die drei API-Header aus dem
[mxpanel](https://panel.mxroute.com/api-keys.php) unter *API Keys*:

- **X-Server** – Mailserver-Hostname (z. B. `eagle.mxlogin.com`). Die App lädt die vollständige Liste live von der [MXroute-Statusseite](https://status.mxroute.com) (`/api/status`) und bietet sie als Auswahlliste an; der Hostname kann weiterhin frei eingegeben werden.
- **X-Username** – DirectAdmin-Benutzername
- **X-API-Key** – der erstellte API-Key

Auf Wunsch werden die Zugangsdaten lokal gespeichert. Der API-Key wird dabei mit der
Windows-DPAPI (nur für den angemeldeten Windows-Benutzer entschlüsselbar) verschlüsselt
unter `%APPDATA%\MxRouteManager\settings.json` abgelegt.

### Voraussetzungen (zur Ausführung)

Die App wird **framework-abhängig** ausgeliefert – die `.exe` ist dadurch nur ~0,5 MB groß,
setzt aber eine installierte Runtime voraus:

- **Windows x64** (Windows 10 / 11)
- **[.NET 8 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0)**
  – der Eintrag **„.NET Desktop Runtime 8.x“** (nicht nur „.NET Runtime“, WPF benötigt die Desktop-Variante)

Prüfen mit `dotnet --list-runtimes` (es muss `Microsoft.WindowsDesktop.App 8.x` erscheinen).
Fehlt sie: über den Link oben oder `winget install Microsoft.DotNet.DesktopRuntime.8` installieren.

### Build

Voraussetzung zum Bauen: [.NET 8 SDK](https://dotnet.microsoft.com/download).

```powershell
# Entwicklungs-Build
dotnet build src/MxRouteManager/MxRouteManager.csproj -c Debug

# Schlanke EXE (Standard, framework-abhängig, ~0,5 MB) -> benötigt .NET 8 Desktop Runtime
dotnet publish src/MxRouteManager/MxRouteManager.csproj -c Release -o publish
```

Ergebnis: `publish/MxRouteManager.exe` – eine einzelne, portable `.exe` für Windows x64.

**Alternative:** eigenständige EXE ohne Runtime-Abhängigkeit (größer, ~60 MB):

```powershell
dotnet publish src/MxRouteManager/MxRouteManager.csproj -c Release -o publish-standalone `
  --self-contained true -p:EnableCompressionInSingleFile=true
```

### Technik

- **WPF / .NET 8** (`net8.0-windows`), **MVVM** mit [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
- `HttpClient` + `System.Text.Json` für die API-Anbindung
- DPAPI (`System.Security.Cryptography.ProtectedData`) für die Schlüssel-Verschlüsselung
- Eigene Lokalisierung mit Laufzeit-Umschaltung (DE/EN)

### Lizenz

[MIT-Lizenz](LICENSE) – frei nutzbar, veränderbar und weitergebbar, ohne Gewährleistung.
Verwendete Bibliotheken stehen ebenfalls unter der MIT-Lizenz.

---

<a name="english"></a>

## English

A native Windows desktop application (WPF, .NET 8) for managing
[MXroute](https://mxroute.com) email hosting through the official
[MXroute API](https://api.mxroute.com/docs).

**All non-reseller features** of the API are integrated. Reseller features
(user and package management) are intentionally omitted.

The user interface is **bilingual (German/English)** and can be switched at any time
via the **DE/EN toggle** in the sidebar – without a restart. Your choice is saved.

> This is an **unofficial** client. The project is not affiliated with or endorsed by
> MXroute. "MXroute" is a trademark of its respective owners and is used here only to
> describe compatibility.

### Features

| Area | Features |
|------|----------|
| **Dashboard** | Overall storage usage, breakdown (email/web/DB/backups/other), storage per mailbox, grace-period warning |
| **Domains** | List, add, view details, delete, toggle mail hosting |
| **Email accounts** | List, create (with password generator), edit (password/quota/limit), delete |
| **Forwarders** | List, create (multiple destinations, `:blackhole:` / `:fail:`), delete |
| **Domain pointers** | List, create (alias or redirect), delete |
| **Spam filter** | Set score threshold, manage whitelist & blacklist |
| **Catch-all** | `fail` / `blackhole` / forward to address |
| **DNS info** | MX, SPF, DKIM and verification records (read-only, copyable) |
| **Verification key** | Retrieve and copy the TXT record needed to add new domains |

### Credentials

The app requires the three API headers found in the
[mxpanel](https://panel.mxroute.com/api-keys.php) under *API Keys*:

- **X-Server** – mail server hostname (e.g. `eagle.mxlogin.com`). The app loads the complete list live from the [MXroute status page](https://status.mxroute.com) (`/api/status`) and offers it as a dropdown; the hostname can still be typed manually.
- **X-Username** – DirectAdmin username
- **X-API-Key** – the API key you created

Optionally, credentials are stored locally. The API key is encrypted using the
Windows DPAPI (decryptable only by the signed-in Windows user) at
`%APPDATA%\MxRouteManager\settings.json`.

### Requirements (to run)

The app ships **framework-dependent** – the `.exe` is only ~0.5 MB, but requires an
installed runtime:

- **Windows x64** (Windows 10 / 11)
- **[.NET 8 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0)**
  – the **".NET Desktop Runtime 8.x"** entry (not just ".NET Runtime"; WPF needs the Desktop variant)

Check with `dotnet --list-runtimes` (a line `Microsoft.WindowsDesktop.App 8.x` must appear).
If missing, install it via the link above or `winget install Microsoft.DotNet.DesktopRuntime.8`.

### Build

Prerequisite to build: [.NET 8 SDK](https://dotnet.microsoft.com/download).

```powershell
# Development build
dotnet build src/MxRouteManager/MxRouteManager.csproj -c Debug

# Slim EXE (default, framework-dependent, ~0.5 MB) -> requires .NET 8 Desktop Runtime
dotnet publish src/MxRouteManager/MxRouteManager.csproj -c Release -o publish
```

Result: `publish/MxRouteManager.exe` – a single, portable `.exe` for Windows x64.

**Alternative:** self-contained EXE with no runtime dependency (larger, ~60 MB):

```powershell
dotnet publish src/MxRouteManager/MxRouteManager.csproj -c Release -o publish-standalone `
  --self-contained true -p:EnableCompressionInSingleFile=true
```

### Tech

- **WPF / .NET 8** (`net8.0-windows`), **MVVM** with [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
- `HttpClient` + `System.Text.Json` for the API integration
- DPAPI (`System.Security.Cryptography.ProtectedData`) for key encryption
- Custom localization with runtime language switching (DE/EN)

### License

[MIT License](LICENSE) – free to use, modify and distribute, without warranty.
The libraries used are also licensed under MIT.

---

## Projektstruktur / Project structure

```
src/MxRouteManager/
├── Localization/     Loc-Manager, {loc:Tr}-Markup, DE/EN-Katalog
├── Models/           API data models (DTOs)
├── Services/         API client, status-server list, settings store, dialog service
├── ViewModels/       MVVM view models (one per page)
├── Views/            XAML views (UserControls)
├── Dialogs/          Input dialogs (account, forwarder, pointer, text)
├── Converters/       WPF value converters
├── Themes/           Dark theme (colors & control styles)
├── MainWindow.xaml   Shell with sidebar navigation
└── App.xaml          Resources & VM→View mapping

docs/openapi.yaml     Reference: OpenAPI specification of the MXroute API
```

## Rate-Limits / Rate limits

Die API begrenzt Anfragen (100 Lese- bzw. 20 Schreibvorgänge pro Minute). ·
The API limits requests (100 reads / 20 writes per minute). Bei Überschreitung /
on exceeding: `RATE_LIMITED` – kurz warten / wait briefly and retry.
