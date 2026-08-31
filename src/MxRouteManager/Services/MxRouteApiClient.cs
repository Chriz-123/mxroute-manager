using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MxRouteManager.Localization;
using MxRouteManager.Models;

namespace MxRouteManager.Services;

/// <summary>
/// HTTP-Client fuer die MXroute-API. Deckt alle Nicht-Reseller-Endpunkte ab.
/// </summary>
public sealed class MxRouteApiClient
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public string Server { get; private set; } = "";
    public string Username { get; private set; } = "";
    private string _apiKey = "";

    public MxRouteApiClient()
    {
        _http = new HttpClient { BaseAddress = new Uri("https://api.mxroute.com"), Timeout = TimeSpan.FromSeconds(30) };
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void SetCredentials(string server, string username, string apiKey)
    {
        Server = server.Trim();
        Username = username.Trim();
        _apiKey = apiKey.Trim();
    }

    private HttpRequestMessage BuildRequest(HttpMethod method, string path, object? body)
    {
        var req = new HttpRequestMessage(method, path);
        req.Headers.Add("X-Server", Server);
        req.Headers.Add("X-Username", Username);
        req.Headers.Add("X-API-Key", _apiKey);
        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body, JsonOpts);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }
        return req;
    }

    // ---------- Kern-Aufrufe ----------

    /// <summary>Fuehrt einen Aufruf aus und liefert das rohe JSON-Dokument zurueck (oder null bei 204).</summary>
    private async Task<JsonDocument?> SendAsync(HttpMethod method, string path, object? body = null, CancellationToken ct = default)
    {
        HttpResponseMessage resp;
        try
        {
            resp = await _http.SendAsync(BuildRequest(method, path, body), ct).ConfigureAwait(false);
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new ApiException(Loc.T("Api_Timeout"));
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(Loc.T("Api_Network", ex.Message));
        }

        var text = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        if (resp.StatusCode == HttpStatusCode.NoContent || string.IsNullOrWhiteSpace(text))
        {
            if (resp.IsSuccessStatusCode) return null;
            throw new ApiException(Loc.T("Api_HttpError2", (int)resp.StatusCode, resp.ReasonPhrase ?? ""), (int)resp.StatusCode);
        }

        JsonDocument doc;
        try { doc = JsonDocument.Parse(text); }
        catch (JsonException)
        {
            if (resp.IsSuccessStatusCode) throw new ApiException(Loc.T("Api_InvalidJson"));
            throw new ApiException(Loc.T("Api_HttpError2", (int)resp.StatusCode, resp.ReasonPhrase ?? ""), (int)resp.StatusCode);
        }

        if (!resp.IsSuccessStatusCode)
        {
            string message = Loc.T("Api_HttpError1", (int)resp.StatusCode);
            string? code = null, field = null;
            if (doc.RootElement.TryGetProperty("error", out var err) && err.ValueKind == JsonValueKind.Object)
            {
                if (err.TryGetProperty("message", out var m) && m.ValueKind == JsonValueKind.String) message = m.GetString()!;
                if (err.TryGetProperty("code", out var c) && c.ValueKind == JsonValueKind.String) code = c.GetString();
                if (err.TryGetProperty("field", out var f) && f.ValueKind == JsonValueKind.String) field = f.GetString();
            }
            doc.Dispose();
            throw new ApiException(message, (int)resp.StatusCode, code, field);
        }

        return doc;
    }

    /// <summary>Liest das "data"-Feld einer Standard-Antwort in T ein.</summary>
    private async Task<T?> GetDataAsync<T>(HttpMethod method, string path, object? body = null, CancellationToken ct = default)
    {
        using var doc = await SendAsync(method, path, body, ct).ConfigureAwait(false);
        if (doc is null) return default;
        if (doc.RootElement.TryGetProperty("data", out var data))
            return data.Deserialize<T>(JsonOpts);
        // Manche Endpunkte liefern das Objekt direkt (z.B. /quota).
        return doc.RootElement.Deserialize<T>(JsonOpts);
    }

    private async Task SendNoContentAsync(HttpMethod method, string path, object? body = null, CancellationToken ct = default)
    {
        using var _ = await SendAsync(method, path, body, ct).ConfigureAwait(false);
    }

    // ---------- Verbindungstest ----------
    public async Task VerifyConnectionAsync(CancellationToken ct = default)
        => await GetDomainsAsync(ct).ConfigureAwait(false);

    // ---------- Domains ----------
    public async Task<List<string>> GetDomainsAsync(CancellationToken ct = default)
        => await GetDataAsync<List<string>>(HttpMethod.Get, "/domains", ct: ct).ConfigureAwait(false) ?? new();

    public Task<Domain?> GetDomainAsync(string domain, CancellationToken ct = default)
        => GetDataAsync<Domain>(HttpMethod.Get, $"/domains/{Uri.EscapeDataString(domain)}", ct: ct);

    public Task CreateDomainAsync(string domain, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Post, "/domains", new { domain }, ct);

    public Task DeleteDomainAsync(string domain, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Delete, $"/domains/{Uri.EscapeDataString(domain)}", ct: ct);

    public Task SetMailStatusAsync(string domain, bool enabled, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Patch, $"/domains/{Uri.EscapeDataString(domain)}/mail-status", new { enabled }, ct);

    // ---------- Domain-Pointer ----------
    public async Task<List<DomainPointer>> GetPointersAsync(string domain, CancellationToken ct = default)
        => await GetDataAsync<List<DomainPointer>>(HttpMethod.Get, $"/domains/{Uri.EscapeDataString(domain)}/pointers", ct: ct).ConfigureAwait(false) ?? new();

    public Task CreatePointerAsync(string domain, string pointer, bool alias, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Post, $"/domains/{Uri.EscapeDataString(domain)}/pointers", new { pointer, alias }, ct);

    public Task DeletePointerAsync(string domain, string pointer, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Delete, $"/domains/{Uri.EscapeDataString(domain)}/pointers/{Uri.EscapeDataString(pointer)}", ct: ct);

    // ---------- E-Mail-Konten ----------
    public async Task<List<EmailAccount>> GetEmailAccountsAsync(string domain, CancellationToken ct = default)
        => await GetDataAsync<List<EmailAccount>>(HttpMethod.Get, $"/domains/{Uri.EscapeDataString(domain)}/email-accounts", ct: ct).ConfigureAwait(false) ?? new();

    public Task<EmailAccount?> GetEmailAccountAsync(string domain, string user, CancellationToken ct = default)
        => GetDataAsync<EmailAccount>(HttpMethod.Get, $"/domains/{Uri.EscapeDataString(domain)}/email-accounts/{Uri.EscapeDataString(user)}", ct: ct);

    public Task CreateEmailAccountAsync(string domain, string username, string password, int quota, int limit, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Post, $"/domains/{Uri.EscapeDataString(domain)}/email-accounts",
            new { username, password, quota, limit }, ct);

    public Task UpdateEmailAccountAsync(string domain, string user, string? password, int? quota, int? limit, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(password)) body["password"] = password;
        if (quota.HasValue) body["quota"] = quota.Value;
        if (limit.HasValue) body["limit"] = limit.Value;
        return SendNoContentAsync(HttpMethod.Patch, $"/domains/{Uri.EscapeDataString(domain)}/email-accounts/{Uri.EscapeDataString(user)}", body, ct);
    }

    public Task DeleteEmailAccountAsync(string domain, string user, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Delete, $"/domains/{Uri.EscapeDataString(domain)}/email-accounts/{Uri.EscapeDataString(user)}", ct: ct);

    // ---------- Weiterleitungen ----------
    public async Task<List<Forwarder>> GetForwardersAsync(string domain, CancellationToken ct = default)
        => await GetDataAsync<List<Forwarder>>(HttpMethod.Get, $"/domains/{Uri.EscapeDataString(domain)}/forwarders", ct: ct).ConfigureAwait(false) ?? new();

    public Task CreateForwarderAsync(string domain, string alias, IEnumerable<string> destinations, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Post, $"/domains/{Uri.EscapeDataString(domain)}/forwarders",
            new { alias, destinations = destinations.ToArray() }, ct);

    public Task DeleteForwarderAsync(string domain, string alias, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Delete, $"/domains/{Uri.EscapeDataString(domain)}/forwarders/{Uri.EscapeDataString(alias)}", ct: ct);

    // ---------- Spam ----------
    public Task<SpamSettings?> GetSpamSettingsAsync(string domain, CancellationToken ct = default)
        => GetDataAsync<SpamSettings>(HttpMethod.Get, $"/domains/{Uri.EscapeDataString(domain)}/spam/settings", ct: ct);

    public Task UpdateSpamSettingsAsync(string domain, int highScore, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Patch, $"/domains/{Uri.EscapeDataString(domain)}/spam/settings", new { high_score = highScore }, ct);

    public async Task<List<string>> GetWhitelistAsync(string domain, CancellationToken ct = default)
        => await GetDataAsync<List<string>>(HttpMethod.Get, $"/domains/{Uri.EscapeDataString(domain)}/spam/whitelist", ct: ct).ConfigureAwait(false) ?? new();

    public Task AddWhitelistAsync(string domain, string entry, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Post, $"/domains/{Uri.EscapeDataString(domain)}/spam/whitelist", new { entry }, ct);

    public Task RemoveWhitelistAsync(string domain, string entry, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Delete, $"/domains/{Uri.EscapeDataString(domain)}/spam/whitelist/{Uri.EscapeDataString(entry)}", ct: ct);

    public async Task<List<string>> GetBlacklistAsync(string domain, CancellationToken ct = default)
        => await GetDataAsync<List<string>>(HttpMethod.Get, $"/domains/{Uri.EscapeDataString(domain)}/spam/blacklist", ct: ct).ConfigureAwait(false) ?? new();

    public Task AddBlacklistAsync(string domain, string entry, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Post, $"/domains/{Uri.EscapeDataString(domain)}/spam/blacklist", new { entry }, ct);

    public Task RemoveBlacklistAsync(string domain, string entry, CancellationToken ct = default)
        => SendNoContentAsync(HttpMethod.Delete, $"/domains/{Uri.EscapeDataString(domain)}/spam/blacklist/{Uri.EscapeDataString(entry)}", ct: ct);

    // ---------- DNS ----------
    public Task<DnsInfo?> GetDnsInfoAsync(string domain, CancellationToken ct = default)
        => GetDataAsync<DnsInfo>(HttpMethod.Get, $"/domains/{Uri.EscapeDataString(domain)}/dns", ct: ct);

    // ---------- Catch-All ----------
    public Task<CatchAll?> GetCatchAllAsync(string domain, CancellationToken ct = default)
        => GetDataAsync<CatchAll>(HttpMethod.Get, $"/domains/{Uri.EscapeDataString(domain)}/catch-all", ct: ct);

    public Task SetCatchAllAsync(string domain, string type, string? address, CancellationToken ct = default)
    {
        object body = type == "address" ? new { type, address } : new { type };
        return SendNoContentAsync(HttpMethod.Patch, $"/domains/{Uri.EscapeDataString(domain)}/catch-all", body, ct);
    }

    // ---------- Quota ----------
    public Task<QuotaInfo?> GetQuotaAsync(CancellationToken ct = default)
        => GetDataAsync<QuotaInfo>(HttpMethod.Get, "/quota", ct: ct);

    public Task<EmailQuotaInfo?> GetEmailQuotaAsync(CancellationToken ct = default)
        => GetDataAsync<EmailQuotaInfo>(HttpMethod.Get, "/quota/email", ct: ct);

    // ---------- Account ----------
    public Task<VerificationKey?> GetVerificationKeyAsync(CancellationToken ct = default)
        => GetDataAsync<VerificationKey>(HttpMethod.Get, "/verification-key", ct: ct);
}
