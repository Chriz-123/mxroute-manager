using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using MxRouteManager.Models;

namespace MxRouteManager.Services;

/// <summary>
/// Laedt die oeffentliche Serverliste von der MXroute-Statusseite
/// (<c>https://status.mxroute.com/api/status</c>). Die REST-API unter
/// api.mxroute.com bietet keinen Server-Endpunkt; X-Server muss der Nutzer
/// selbst angeben. Die Statusseite ist die vollstaendige, von MXroute
/// gepflegte Quelle.
/// </summary>
public sealed class MxRouteStatusClient
{
    private const string StatusUrl = "https://status.mxroute.com/api/status";

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly HttpClient Http = CreateClient();

    /// <summary>
    /// Gemeinsame Dienste auf der Statusseite, die keine Konten-Mailserver
    /// (und damit keine X-Server-Werte) sind.
    /// </summary>
    internal static readonly HashSet<string> NonAccountHostnames = new(StringComparer.OrdinalIgnoreCase)
    {
        "mail.mxlogin.com",      // Crossbox-Webmail
        "webmail.mxroute.com",   // zentrales Webmail
        "zmta"                   // Outbound-MTA, kein Hostname
    };

    /// <summary>
    /// Eingebetteter Stand der Konten-Server, falls die Statusseite nicht
    /// erreichbar ist. Wird beim naechsten erfolgreichen Abruf ersetzt.
    /// </summary>
    public static readonly IReadOnlyList<MailServer> FallbackServers = BuildFallback();

    private readonly HttpClient _http;

    public MxRouteStatusClient() : this(Http) { }

    internal MxRouteStatusClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<MailServer>> GetAccountServersAsync(CancellationToken ct = default)
    {
        using var resp = await _http.GetAsync(StatusUrl, ct).ConfigureAwait(false);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        var raw = JsonSerializer.Deserialize<List<StatusServerDto>>(json, JsonOpts) ?? new();
        var list = SelectAccountServers(raw);
        return list.Count > 0 ? list : FallbackServers;
    }

    /// <summary>
    /// Versucht die Live-Liste; bei jedem Fehler (Netzwerk, HTTP, JSON)
    /// wird die eingebettete Fallback-Liste geliefert.
    /// </summary>
    public async Task<(IReadOnlyList<MailServer> Servers, bool FromLive)> GetAccountServersOrFallbackAsync(
        CancellationToken ct = default)
    {
        try
        {
            var live = await GetAccountServersAsync(ct).ConfigureAwait(false);
            return (live, true);
        }
        catch
        {
            return (FallbackServers, false);
        }
    }

    internal static List<MailServer> SelectAccountServers(IEnumerable<StatusServerDto> raw)
    {
        return raw
            .Where(s => !string.IsNullOrWhiteSpace(s.Hostname))
            .Select(s => new MailServer
            {
                Hostname = s.Hostname.Trim(),
                DisplayName = string.IsNullOrWhiteSpace(s.DisplayName) ? s.Hostname.Trim() : s.DisplayName.Trim(),
                PanelType = (s.PanelType ?? "").Trim()
            })
            .Where(s => s.Hostname.Contains('.'))
            .Where(s => !NonAccountHostnames.Contains(s.Hostname))
            .GroupBy(s => s.Hostname, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .OrderBy(s => s.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(s => s.Hostname, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static HttpClient CreateClient()
    {
        var http = new HttpClient { Timeout = TimeSpan.FromSeconds(12) };
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        http.DefaultRequestHeaders.UserAgent.ParseAdd("MxRouteManager/1.1");
        return http;
    }

    private static IReadOnlyList<MailServer> BuildFallback()
    {
        (string Name, string Host)[] rows =
        [
            ("Arrow", "arrow.mxrouting.net"),
            ("Aus", "aus.mxroute.com"),
            ("Banshee", "banshee.mxlogin.com"),
            ("Blizzard", "blizzard.mxrouting.net"),
            ("Chocobo", "chocobo.mxrouting.net"),
            ("Eagle", "eagle.mxlogin.com"),
            ("Echo", "echo.mxrouting.net"),
            ("Everest", "everest.mxrouting.net"),
            ("Friday", "friday.mxlogin.com"),
            ("Fusion", "fusion.mxrouting.net"),
            ("Glacier", "glacier.mxrouting.net"),
            ("Heracles", "heracles.mxrouting.net"),
            ("London", "london.mxroute.com"),
            ("Longhorn", "longhorn.mxrouting.net"),
            ("Lucy", "lucy.mxrouting.net"),
            ("Monday", "monday.mxrouting.net"),
            ("Moose", "moose.mxrouting.net"),
            ("Ocean", "ocean.mxroute.com"),
            ("Pixel", "pixel.mxrouting.net"),
            ("Redbull", "redbull.mxrouting.net"),
            ("Safari", "safari.mxrouting.net"),
            ("Shadow", "shadow.mxrouting.net"),
            ("Sunfire", "sunfire.mxrouting.net"),
            ("Taylor", "taylor.mxrouting.net"),
            ("Tuesday", "tuesday.mxrouting.net"),
            ("Wednesday", "wednesday.mxrouting.net"),
            ("Witcher", "witcher.mxrouting.net"),
        ];

        return rows
            .Select(r => new MailServer { DisplayName = r.Name, Hostname = r.Host })
            .ToList();
    }

    internal sealed class StatusServerDto
    {
        [JsonPropertyName("hostname")] public string Hostname { get; set; } = "";
        [JsonPropertyName("display_name")] public string DisplayName { get; set; } = "";
        [JsonPropertyName("panel_type")] public string PanelType { get; set; } = "";
    }
}
