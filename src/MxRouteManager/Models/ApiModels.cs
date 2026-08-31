using System.Text.Json.Serialization;

namespace MxRouteManager.Models;

/// <summary>Standard-Antworthuelle der API: { success, data } bzw. { success, error }.</summary>
public sealed class ApiResponse<T>
{
    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("data")] public T? Data { get; set; }
    [JsonPropertyName("error")] public ApiError? Error { get; set; }
}

public sealed class ApiError
{
    [JsonPropertyName("code")] public string? Code { get; set; }
    [JsonPropertyName("message")] public string? Message { get; set; }
    [JsonPropertyName("field")] public string? Field { get; set; }
}

public sealed class Domain
{
    [JsonPropertyName("domain")] public string Name { get; set; } = "";
    [JsonPropertyName("mail_hosting")] public bool MailHosting { get; set; }
    [JsonPropertyName("ssl_enabled")] public bool SslEnabled { get; set; }
    [JsonPropertyName("pointers")] public List<string> Pointers { get; set; } = new();
}

public sealed class EmailAccount
{
    [JsonPropertyName("username")] public string Username { get; set; } = "";
    [JsonPropertyName("email")] public string Email { get; set; } = "";
    [JsonPropertyName("quota")] public long Quota { get; set; }
    [JsonPropertyName("usage")] public double Usage { get; set; }
    [JsonPropertyName("limit")] public int Limit { get; set; }
    [JsonPropertyName("sent")] public int Sent { get; set; }
    [JsonPropertyName("suspended")] public bool Suspended { get; set; }

    [JsonIgnore] public string QuotaDisplay => Quota == 0 ? "Unbegrenzt" : $"{Quota} MB";
    [JsonIgnore] public string UsageDisplay => $"{Usage:0.#} MB";
    [JsonIgnore] public string SentDisplay => $"{Sent} / {Limit}";
    [JsonIgnore] public string StatusDisplay => Suspended ? "Gesperrt" : "Aktiv";
}

public sealed class Forwarder
{
    [JsonPropertyName("alias")] public string Alias { get; set; } = "";
    [JsonPropertyName("email")] public string Email { get; set; } = "";
    [JsonPropertyName("destinations")] public List<string> Destinations { get; set; } = new();

    [JsonIgnore] public string DestinationsDisplay => string.Join(", ", Destinations);
}

public sealed class DomainPointer
{
    [JsonPropertyName("pointer")] public string Pointer { get; set; } = "";
    [JsonPropertyName("type")] public string Type { get; set; } = "";
    [JsonPropertyName("target")] public string Target { get; set; } = "";

    [JsonIgnore] public string TypeDisplay => Type == "redirect" ? "Weiterleitung" : "Alias";
}

public sealed class CatchAll
{
    [JsonPropertyName("type")] public string Type { get; set; } = "fail";
    [JsonPropertyName("address")] public string? Address { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
}

public sealed class SpamSettings
{
    [JsonPropertyName("high_score")] public int HighScore { get; set; }
}

// ---------- DNS ----------
public sealed class DnsInfo
{
    [JsonPropertyName("mx_records")] public List<MxRecord> MxRecords { get; set; } = new();
    [JsonPropertyName("spf")] public DnsRecord? Spf { get; set; }
    [JsonPropertyName("dkim")] public DnsRecord? Dkim { get; set; }
    [JsonPropertyName("verification")] public DnsRecord? Verification { get; set; }
}

public sealed class MxRecord
{
    [JsonPropertyName("priority")] public int Priority { get; set; }
    [JsonPropertyName("hostname")] public string Hostname { get; set; } = "";
    [JsonPropertyName("description")] public string? Description { get; set; }
}

public sealed class DnsRecord
{
    [JsonPropertyName("type")] public string? Type { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("value")] public string? Value { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
}

// ---------- Quota ----------
public sealed class QuotaInfo
{
    [JsonPropertyName("username")] public string? Username { get; set; }
    [JsonPropertyName("total_used")] public long TotalUsed { get; set; }
    [JsonPropertyName("total_limit")] public long TotalLimit { get; set; }
    [JsonPropertyName("percent_used")] public double PercentUsed { get; set; }
    [JsonPropertyName("breakdown")] public QuotaBreakdown? Breakdown { get; set; }
    [JsonPropertyName("grace_period")] public GracePeriod? GracePeriod { get; set; }
    [JsonPropertyName("updated_at")] public string? UpdatedAt { get; set; }
}

public sealed class QuotaBreakdown
{
    [JsonPropertyName("email")] public long Email { get; set; }
    [JsonPropertyName("web")] public long Web { get; set; }
    [JsonPropertyName("databases")] public long Databases { get; set; }
    [JsonPropertyName("backups")] public long Backups { get; set; }
    [JsonPropertyName("other")] public long Other { get; set; }
}

public sealed class GracePeriod
{
    [JsonPropertyName("days_remaining")] public int DaysRemaining { get; set; }
    [JsonPropertyName("deadline")] public string? Deadline { get; set; }
}

public sealed class EmailQuotaInfo
{
    [JsonPropertyName("username")] public string? Username { get; set; }
    [JsonPropertyName("accounts")] public List<EmailQuotaAccount> Accounts { get; set; } = new();
}

public sealed class EmailQuotaAccount
{
    [JsonPropertyName("email_address")] public string EmailAddress { get; set; } = "";
    [JsonPropertyName("size_bytes")] public long SizeBytes { get; set; }
    [JsonPropertyName("updated_at")] public string? UpdatedAt { get; set; }

    [JsonIgnore] public string SizeDisplay => FormatBytes.Humanize(SizeBytes);
}

// ---------- Account ----------
public sealed class VerificationKey
{
    [JsonPropertyName("key")] public string? Key { get; set; }
    [JsonPropertyName("record")] public DnsRecord? Record { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
}

public static class FormatBytes
{
    public static string Humanize(long bytes)
    {
        if (bytes <= 0) return "0 B";
        string[] units = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < units.Length - 1) { order++; len /= 1024; }
        return $"{len:0.##} {units[order]}";
    }
}
